using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Factories;

public interface IImageInfoViewFactory
{
	IImageInfoView GetImageInfoView(IImageFile imageFile);
}
