using FluentAssertions;
using Moq;
using Xunit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

namespace ImageFanReloaded.Avalonia.Tests.CommonTypes.ImageHandling.Implementation;

public class ImageResizerTest
{
    static ImageResizerTest()
	{
		var appBuilderInitializerInstance = AppBuilderInitializer.Instance;
	}
	
	public ImageResizerTest()
    {
		_imageResizeCalculatorMock = new Mock<IImageResizeCalculator>(MockBehavior.Strict);
        
        _imageResizer = new ImageResizer(_imageResizeCalculatorMock.Object);
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
		IImage image = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var resizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		_imageResizeCalculatorMock
			.Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageSize(
				new ImageSize(image.Size.Width, image.Size.Height),
				viewPortSize))
			.Returns(resizedImageSize);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);

		// Assert
		resizedImage.Size.Width.Should().Be(resizedImageSize.Width);
		resizedImage.Size.Height.Should().Be(resizedImageSize.Height);

		var resizedBitmap = resizedImage as Bitmap;
		var outputFileName = GetOutputImageFileName(
			LandscapeImageFileName, viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
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
		IImage image = new Bitmap($"{PortraitImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var resizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		_imageResizeCalculatorMock
			.Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageSize(
				new ImageSize(image.Size.Width, image.Size.Height),
				viewPortSize))
			.Returns(resizedImageSize);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);

		// Assert
		resizedImage.Size.Width.Should().Be(resizedImageSize.Width);
		resizedImage.Size.Height.Should().Be(resizedImageSize.Height);

		var resizedBitmap = resizedImage as Bitmap;
		var outputFileName = GetOutputImageFileName(
			PortraitImageFileName, viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
	}

	#region Private

	private const string LandscapeImageFileName = "Landscape";
	private const string PortraitImageFileName = "Portrait";

	private const string InputFileExtension = ".jpg";
	private const string OutputFileExtension = ".png";

	private readonly Mock<IImageResizeCalculator> _imageResizeCalculatorMock;

    private readonly ImageResizer _imageResizer;

	private static string GetOutputImageFileName(
		string imageFileName, ImageSize viewPortSize, ImageSize resizedImageSize)
		=> $"{imageFileName}-ViewPort-{viewPortSize.Width}x{viewPortSize.Height}-Resized-{resizedImageSize.Width}x{resizedImageSize.Height}{OutputFileExtension}";

    #endregion
}
