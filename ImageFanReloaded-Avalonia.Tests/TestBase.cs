using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ImageFanReloaded.Avalonia.Tests;

public abstract class TestBase
{
	static TestBase()
	{
		var appBuilderInitializerInstance = AppBuilderInitializer.Instance;
	}

	#region Protected

	protected const string OutputFileExtension = ".png";

	protected void SaveImageToDisc(IImage image, string imagePath)
	{
		var bitmap = image as Bitmap;
		bitmap.Save(imagePath);
	}

	#endregion
}
