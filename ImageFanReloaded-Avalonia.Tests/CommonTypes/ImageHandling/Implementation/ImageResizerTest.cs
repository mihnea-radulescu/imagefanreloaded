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
	public void CreateResizedImage_LandscapeImage_ViewPortSizeIs3840x2160_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(3840, 2160);
		var resizedImageSize = new ImageSize(1920, 1080);

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
		var outputFileName = GetOutputImageFileName(viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
	}

	[Fact]
	public void CreateResizedImage_LandscapeImage_ViewPortSizeIs1920x1080_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(1920, 1080);
		var resizedImageSize = new ImageSize(1920, 1080);

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
		var outputFileName = GetOutputImageFileName(viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
	}

	[Fact]
    public void CreateResizedImage_LandscapeImage_ViewPortSizeIs1280x720_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(1280, 720);
		var resizedImageSize = new ImageSize(1280, 720);

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
		var outputFileName = GetOutputImageFileName(viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
	}

	[Fact]
	public void CreateResizedImage_LandscapeImage_ViewPortSizeIs960x540_ReturnsCorrectlyResizedImage()
	{
		// Arrange
		IImage image = new Bitmap($"{LandscapeImageFileName}{InputFileExtension}");

		var viewPortSize = new ImageSize(960, 540);
		var resizedImageSize = new ImageSize(960, 540);

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
		var outputFileName = GetOutputImageFileName(viewPortSize, resizedImageSize);
		resizedBitmap.Save(outputFileName);
	}

	#region Private

	private const string LandscapeImageFileName = "Landscape";
	private const string InputFileExtension = ".jpg";
	private const string OutputFileExtension = ".png";

	private readonly Mock<IImageResizeCalculator> _imageResizeCalculatorMock;

    private readonly ImageResizer _imageResizer;

	private static string GetOutputImageFileName(
		ImageSize viewPortSize, ImageSize resizedImageSize)
		=> $"{LandscapeImageFileName}-{viewPortSize.Width}x{viewPortSize.Height}-Resized-{resizedImageSize.Width}x{resizedImageSize.Height}{OutputFileExtension}";

    #endregion
}
