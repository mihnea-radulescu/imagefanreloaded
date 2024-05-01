using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IDiscQueryEngine
{
	Task<IReadOnlyCollection<FileSystemEntryInfo>> GetRootFolders();

	Task<IReadOnlyCollection<FileSystemEntryInfo>> GetSubFolders(string folderPath);

	Task<IReadOnlyCollection<IImageFile>> GetImageFiles(string folderPath);
}
