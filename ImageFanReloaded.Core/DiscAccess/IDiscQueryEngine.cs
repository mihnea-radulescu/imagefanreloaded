using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	Task<IReadOnlyList<FileSystemEntryInfo>> GetRootFolders();

	Task<IReadOnlyList<FileSystemEntryInfo>> GetSubFolders(string folderPath);
	
	Task<FileSystemEntryInfo> GetFileSystemEntryInfo(string folderPath);

	Task<IReadOnlyList<IImageFile>> GetImageFiles(string folderPath, bool recursive);
}
