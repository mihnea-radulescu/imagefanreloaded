using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathContainer : IInputPathContainer
{
	public InputPathContainer(
		IGlobalParameters globalParameters,
		IDiscQueryEngine discQueryEngine,
		string? inputPath)
	{
		_discQueryEngine = discQueryEngine;
		_nameComparison = globalParameters.NameComparer.ToStringComparison();
		
		InputPath = null;
		_shouldProcessInputPath = false;
		
		if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath) && Directory.Exists(inputPath))
		{
			InputPath = Path.GetFullPath(inputPath);
			_shouldProcessInputPath = true;
		}
	}
	
	public string? InputPath { get; }

	public bool ShouldProcessInputPath() => _shouldProcessInputPath;
	
	public void DisableProcessInputPath()
	{
		_shouldProcessInputPath = false;
	}

	public async Task<FileSystemEntryInfo> GetFileSystemEntryInfo()
		=> await _discQueryEngine.GetFileSystemEntryInfo(InputPath!);

	public async Task<FileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(
		IReadOnlyCollection<FileSystemEntryInfo> folders)
		=> await Task.Run(() =>
			{
				var matchingFileSystemEntryInfo = folders
					.Where(aFolder => InputPath!.Contains(aFolder.Path, _nameComparison))
					.Select(aFolder => new
					{ 
						Folder = aFolder,
						Length = aFolder.Path.Length
					})
					.OrderByDescending(aFolderWithLength => aFolderWithLength.Length)
					.Select(aFolderWithLength => aFolderWithLength.Folder)
					.FirstOrDefault();

				return matchingFileSystemEntryInfo;
			});
	
	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	
	private readonly StringComparison _nameComparison;
	
	private volatile bool _shouldProcessInputPath;

	#endregion
}
