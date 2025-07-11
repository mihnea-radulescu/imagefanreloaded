using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Factories;

public interface IImageEditViewFactory
{
	IImageEditView GetImageEditView(IContentTabItem contentTabItem, IImageFile imageFile);
}
