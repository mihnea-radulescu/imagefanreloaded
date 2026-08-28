using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;

namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IEditableImageFactory
{
	Task<IEditableImage?> CreateEditableImage(IImageFileData imageFileData);
}
