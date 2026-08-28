using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngine : IDiscQueryEngine
{
	public DiscQueryEngine(IDiscQueryEngineFileSystem discQueryEngineFileSystem)
	{
		_discQueryEngineFileSystem = discQueryEngineFileSystem;
	}

	public async Task BuildSkipRecursionFolderPaths()
		=> await Task.Run(
			() => _discQueryEngineFileSystem.BuildSkipRecursionFolderPaths());

	public async Task<IReadOnlyList<IFileSystemEntryInfo>> GetRootFolders(
		ITabOptions tabOptions)
		=> await Task.Run(() => _discQueryEngineFileSystem.GetRootFolders(
			tabOptions));

	public async Task<IReadOnlyList<IImageFile>> GetImageFilesDefault(
		string folderPath)
			=> await Task.Run(
				() => _discQueryEngineFileSystem.GetImageFilesDefault(
					folderPath));

	public async Task<IReadOnlyList<IFileSystemEntryInfo>> GetSubFolders(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions)
			=> await Task.Run(() => _discQueryEngineFileSystem.GetSubFolders(
				fileSystemEntryInfo, tabOptions));

	public async Task<IReadOnlyList<IImageFile>> GetImageFiles(
		IFileSystemEntryInfo fileSystemEntryInfo, ITabOptions tabOptions)
			=> await Task.Run(() => _discQueryEngineFileSystem.GetImageFiles(
				fileSystemEntryInfo, tabOptions));

	private readonly IDiscQueryEngineFileSystem _discQueryEngineFileSystem;
}
