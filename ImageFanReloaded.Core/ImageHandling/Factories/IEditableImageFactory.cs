using System.Threading.Tasks;

namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IEditableImageFactory
{
	Task<IEditableImage?> CreateEditableImage(string imageFilePath);
}
