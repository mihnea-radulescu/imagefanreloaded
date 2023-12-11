using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ImageFanReloaded.Avalonia.Tests;

public abstract class TestBase
{
	static TestBase()
	{
		var appBuilderInitializerInstance = AppBuilderInitializer.Instance;
	}

	protected const string OutputFileExtension = ".png";

	protected static void SaveImageToDisc(IImage image, string imagePath)
	{
		var bitmap = (Bitmap)image;
		bitmap.Save(imagePath);
	}
}
