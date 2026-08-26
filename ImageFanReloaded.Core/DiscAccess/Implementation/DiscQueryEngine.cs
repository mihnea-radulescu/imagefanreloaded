using System.Collections.Generic;
using System.Threading.Tasks;
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

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders()
		=> await Task.Run(() => _discQueryEngineFileSystem.GetRootFolders());

	public async Task<IReadOnlyList<IImageFile>> GetImageFilesDefault(
		string folderPath)
			=> await Task.Run(
				() => _discQueryEngineFileSystem.GetImageFilesDefault(
					folderPath));

	public async Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(
		string folderPath, ITabOptions tabOptions)
			=> await Task.Run(() => _discQueryEngineFileSystem.GetSubFolders(
				folderPath, tabOptions));

	public async Task<IReadOnlyList<IImageFile>> GetImageFiles(
		string folderPath, ITabOptions tabOptions)
			=> await Task.Run(() => _discQueryEngineFileSystem.GetImageFiles(
				folderPath, tabOptions));

	private readonly IDiscQueryEngineFileSystem _discQueryEngineFileSystem;
}
