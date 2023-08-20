using FluentAssertions;
using Moq;
using Xunit;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Implementation;

namespace ImageFanReloaded.Avalonia.Tests.CommonTypes.ImageHandling.Implementation;

public class ImageResizerTest
{
    static ImageResizerTest()
	{
		AppBuilder
			.Configure<TestApp>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.SetupWithoutStarting();
	}
	
	public ImageResizerTest()
    {
		_imageResizeCalculatorMock = new Mock<IImageResizeCalculator>(MockBehavior.Strict);
        
        _imageResizer = new ImageResizer(_imageResizeCalculatorMock.Object);
    }

	[Fact]
	public void CreateResizedImage_SameAspectRatio_3840x2160_ReturnsUnresizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{InputFileName}{InputFileExtension}");

		var imageDimensionsToResizeTo = new ImageDimensions(3840, 2160);
		var resizedImageDimensions = new ImageDimensions(1920, 1080);

		_imageResizeCalculatorMock
			.Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageDimensions(
				new ImageDimensions(image.Size.Width, image.Size.Height),
				imageDimensionsToResizeTo))
			.Returns(resizedImageDimensions);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, imageDimensionsToResizeTo);

		// Assert
		resizedImage.Size.Width.Should().Be(resizedImageDimensions.Width);
		resizedImage.Size.Height.Should().Be(resizedImageDimensions.Height);

		var resizedBitmap = resizedImage as Bitmap;
		resizedBitmap.Save(
			$"{InputFileName}-Resized-{resizedImageDimensions.Width}x{resizedImageDimensions.Height}{OutputFileExtension}");
	}

	[Fact]
	public void CreateResizedImage_SameAspectRatio_1920x1080_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{InputFileName}{InputFileExtension}");

		var imageDimensionsToResizeTo = new ImageDimensions(1920, 1080);
		var resizedImageDimensions = new ImageDimensions(1920, 1080);

		_imageResizeCalculatorMock
			.Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageDimensions(
				new ImageDimensions(image.Size.Width, image.Size.Height),
				imageDimensionsToResizeTo))
			.Returns(resizedImageDimensions);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, imageDimensionsToResizeTo);

		// Assert
		resizedImage.Size.Width.Should().Be(resizedImageDimensions.Width);
		resizedImage.Size.Height.Should().Be(resizedImageDimensions.Height);

		var resizedBitmap = resizedImage as Bitmap;
		resizedBitmap.Save(
			$"{InputFileName}-Resized-{resizedImageDimensions.Width}x{resizedImageDimensions.Height}{OutputFileExtension}");
	}

	[Fact]
    public void CreateResizedImage_SameAspectRatio_1280x720_ReturnsCorrectlyResizedImage()
    {
		// Arrange
		IImage image = new Bitmap($"{InputFileName}{InputFileExtension}");

		var imageDimensionsToResizeTo = new ImageDimensions(1280, 720);
		var resizedImageDimensions = new ImageDimensions(1280, 720);

		_imageResizeCalculatorMock
            .Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageDimensions(
                new ImageDimensions(image.Size.Width, image.Size.Height),
				imageDimensionsToResizeTo))
            .Returns(resizedImageDimensions);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, imageDimensionsToResizeTo);

        // Assert
        resizedImage.Size.Width.Should().Be(resizedImageDimensions.Width);
		resizedImage.Size.Height.Should().Be(resizedImageDimensions.Height);

        var resizedBitmap = resizedImage as Bitmap;
        resizedBitmap.Save(
            $"{InputFileName}-Resized-{resizedImageDimensions.Width}x{resizedImageDimensions.Height}{OutputFileExtension}");
	}

	[Fact]
	public void CreateResizedImage_SameAspectRatio_960x540_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{InputFileName}{InputFileExtension}");

		var imageDimensionsToResizeTo = new ImageDimensions(960, 540);
		var resizedImageDimensions = new ImageDimensions(960, 540);

		_imageResizeCalculatorMock
			.Setup(imageResizeCalculator => imageResizeCalculator.GetResizedImageDimensions(
				new ImageDimensions(image.Size.Width, image.Size.Height),
				imageDimensionsToResizeTo))
			.Returns(resizedImageDimensions);

		// Act
		var resizedImage = _imageResizer.CreateResizedImage(image, imageDimensionsToResizeTo);

		// Assert
		resizedImage.Size.Width.Should().Be(resizedImageDimensions.Width);
		resizedImage.Size.Height.Should().Be(resizedImageDimensions.Height);

		var resizedBitmap = resizedImage as Bitmap;
		resizedBitmap.Save(
			$"{InputFileName}-Resized-{resizedImageDimensions.Width}x{resizedImageDimensions.Height}{OutputFileExtension}");
	}

	#region Private

	private const string InputFileName = "Cityscape";
	private const string InputFileExtension = ".jpg";

	private const string OutputFileExtension = ".png";

	private readonly Mock<IImageResizeCalculator> _imageResizeCalculatorMock;

    private readonly ImageResizer _imageResizer;

    #endregion
}
