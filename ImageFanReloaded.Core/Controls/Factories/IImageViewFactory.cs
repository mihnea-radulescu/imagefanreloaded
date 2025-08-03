using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls.Factories;

public interface IImageViewFactory
{
	IImageView GetImageView(ITabOptions tabOptions);
}
