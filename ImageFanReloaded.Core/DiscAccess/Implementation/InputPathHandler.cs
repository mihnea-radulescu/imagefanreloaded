using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathHandler : IInputPathHandler
{
	public InputPathHandler(
		IGlobalParameters globalParameters,
		IDiscQueryEngine discQueryEngine,
		string? inputPath)
	{
		_discQueryEngine = discQueryEngine;
		_nameComparison = globalParameters.NameComparer.ToStringComparison();

		InputPathType = InputPathType.NotSet;

		if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath))
		{
			if (Directory.Exists(inputPath))
			{
				InputPathType = InputPathType.Folder;

				FolderPath = Path.GetFullPath(inputPath);
			}
			else if (File.Exists(inputPath))
			{
				try
				{
					var inputPathFileInfo = new FileInfo(inputPath);

					if (globalParameters.ImageFileExtensions.Contains(inputPathFileInfo.Extension))
					{
						InputPathType = InputPathType.File;

						FolderPath = inputPathFileInfo.DirectoryName;
						FilePath = inputPathFileInfo.FullName;
					}
				}
				catch
				{
				}
			}
		}
	}

	public InputPathType InputPathType { get; }

	public string? FolderPath { get; }
	public string? FilePath { get; }

	public bool CanProcessInputPath() => InputPathType != InputPathType.NotSet;

	public async Task<FileSystemEntryInfo> GetFileSystemEntryInfo()
		=> await _discQueryEngine.GetFileSystemEntryInfo(FolderPath!);

	public async Task<FileSystemEntryInfo?> GetMatchingFileSystemEntryInfo(
		IReadOnlyList<FileSystemEntryInfo> folders)
		=> await Task.Run(() =>
			{
				var matchingFileSystemEntryInfo = folders
					.Where(aFolder => FolderPath!.Contains(aFolder.Path, _nameComparison))
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

	#endregion
}
