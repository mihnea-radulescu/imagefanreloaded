using Avalonia.Media.Imaging;

namespace ImageFanReloaded.Avalonia.Tests;

public abstract class TestBase
{
	static TestBase()
	{
		var appBuilderInitializerInstance = AppBuilderInitializer.Instance;
	}

	protected const string OutputFileExtension = ".png";

	protected static void SaveImageToDisc(Bitmap image, string imagePath)
	{
		image.Save(imagePath);
	}
}
