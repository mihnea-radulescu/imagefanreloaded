using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathContainer : IInputPathContainer
{
	public InputPathContainer(
		IGlobalParameters globalParameters,
		string? inputPath)
	{
		_nameComparison = globalParameters.NameComparer.ToStringComparison();
		
		InputPath = null;
		
		if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath))
		{
			InputPath = Path.GetFullPath(inputPath);
		}

		_hasPopulatedInputPath = false;
	}
	
	public string? InputPath { get; }

	public bool HasPopulatedInputPath
	{
		get => _hasPopulatedInputPath;
		set => _hasPopulatedInputPath = value;
	}

	public bool ShouldPopulateInputPath()
	{
		var shouldPopulateInputPath = !HasPopulatedInputPath && InputPath is not null;
		return shouldPopulateInputPath;
	}

	public FileSystemEntryInfo? GetMatchingFileSystemEntryInfo(IReadOnlyCollection<FileSystemEntryInfo> folders)
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
	}
	
	#region Private
	
	private readonly StringComparison _nameComparison;
	
	private volatile bool _hasPopulatedInputPath;

	#endregion
}
