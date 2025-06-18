using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Factories;

public interface IImageInfoViewFactory
{
	Task<IImageInfoView> GetImageInfoView(IImageFile imageFile);
}
