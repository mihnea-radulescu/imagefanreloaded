using NSubstitute;
using Xunit;
using Avalonia.Media.Imaging;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.Test;

public class ImageResizerTest : TestBase
{
	public ImageResizerTest()
	{
		_imageResizeCalculator = Substitute.For<IImageResizeCalculator>();

		_imageResizer = new ImageResizer(_imageResizeCalculator);
	}

	[Theory]
	[InlineData(3840, 2160, 1920, 1080)]
	[InlineData(1920, 1080, 1920, 1080)]
	[InlineData(1280, 720, 1280, 720)]
	[InlineData(960, 540, 960, 540)]
	public void CreateResizedImage_LandscapeImage_ReturnsCorrectlyResizedImage(
		int viewPortWidth, int viewPortHeight, int resizedImageWidth, int resizedImageHeight)
	{
		// Arrange
		var bitmap = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
		var image = new Image(bitmap, bitmapSize);

		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceResizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		_imageResizeCalculator
			.GetResizedImageSize(
				new ImageSize(image.Size.Width, image.Size.Height), viewPortSize)
			.Returns(referenceResizedImageSize);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize, ImageQuality.Medium);

		// Assert
		Assert.Equal(referenceResizedImageSize.Width, resizedImage.Size.Width);
		Assert.Equal(referenceResizedImageSize.Height, resizedImage.Size.Height);

		var outputFileName = GetOutputImageFileName(
			LandscapeImageFileName, viewPortSize, referenceResizedImageSize);

		SaveImageToDisc(resizedImage.GetBitmap(), outputFileName);
	}

	[Theory]
	[InlineData(3840, 2160, 1080, 1920)]
	[InlineData(1920, 1080, 607, 1080)]
	[InlineData(1280, 720, 405, 720)]
	[InlineData(960, 540, 303, 540)]
	public void CreateResizedImage_PortraitImage_ReturnsCorrectlyResizedImage(
		int viewPortWidth, int viewPortHeight, int resizedImageWidth, int resizedImageHeight)
	{
		// Arrange
		var bitmap = new Bitmap($"{PortraitImageFileName}{InputFileExtension}");
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);
		var image = new Image(bitmap, bitmapSize);

		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceResizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		_imageResizeCalculator
			.GetResizedImageSize(
				new ImageSize(image.Size.Width, image.Size.Height), viewPortSize)
			.Returns(referenceResizedImageSize);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize, ImageQuality.Medium);

		// Assert
		Assert.Equal(referenceResizedImageSize.Width, resizedImage.Size.Width);
		Assert.Equal(referenceResizedImageSize.Height, resizedImage.Size.Height);

		var outputFileName = GetOutputImageFileName(
			PortraitImageFileName, viewPortSize, referenceResizedImageSize);

		SaveImageToDisc(resizedImage.GetBitmap(), outputFileName);
	}

	#region Private

	private const string LandscapeImageFileName = "Landscape";
	private const string PortraitImageFileName = "Portrait";

	private const string InputFileExtension = ".jpg";

	private readonly IImageResizeCalculator _imageResizeCalculator;

	private readonly ImageResizer _imageResizer;

	private static string GetOutputImageFileName(
		string imageFileName, ImageSize viewPortSize, ImageSize resizedImageSize)
		=> $"{imageFileName}-ViewPort-{viewPortSize.Width}x{viewPortSize.Height}-Resized-{resizedImageSize.Width}x{resizedImageSize.Height}{OutputFileExtension}";

	#endregion
}
