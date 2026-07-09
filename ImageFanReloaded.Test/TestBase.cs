using Avalonia.Media.Imaging;

namespace ImageFanReloaded.Test;

public abstract class TestBase
{
	static TestBase()
	{
		var appBuilderInitializerInstance = AppBuilderInitializer.Instance;
	}

	protected const string OutputFileExtension = ".jpg";

	protected static void SaveImageToDisc(Bitmap image, string imagePath)
	{
		image.Save(imagePath, BitmapEncoderOptions);
	}

	private static readonly BitmapEncoderOptions BitmapEncoderOptions =
		new JpegBitmapEncoderOptions();
}
