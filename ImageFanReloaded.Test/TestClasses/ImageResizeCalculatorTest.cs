using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.Test.TestClasses;

public class ImageResizeCalculatorTest
{
	public ImageResizeCalculatorTest()
	{
		_imageResizeCalculator = new ImageResizeCalculator();
	}

	[Theory]
	[InlineData(3840, 2160, 1920, 1080)]
	[InlineData(1920, 1080, 1920, 1080)]
	[InlineData(1280, 720, 1280, 720)]
	[InlineData(960, 540, 960, 540)]
	public void GetDownsizedImageSize_LandscapeImage_ReturnsCorrectResizedImageSize(
		int viewPortWidth, int viewPortHeight, int downsizedImageWidth, int downsizedImageHeight)
	{
		// Arrange
		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceDownsizedImageSize = new ImageSize(downsizedImageWidth, downsizedImageHeight);

		// Act
		var downsizedImageSize = _imageResizeCalculator.GetDownsizedImageSize(LandscapeImageSize, viewPortSize);

		// Assert
		Assert.Equal(referenceDownsizedImageSize, downsizedImageSize);
	}

	[Theory]
	[InlineData(3840, 2160, 1080, 1920)]
	[InlineData(1920, 1080, 607, 1080)]
	[InlineData(1280, 720, 405, 720)]
	[InlineData(960, 540, 303, 540)]
	public void GetDownsizedImageSize_PortraitImage_ReturnsCorrectResizedImageSize(
		int viewPortWidth, int viewPortHeight, int downsizedImageWidth, int downsizedImageHeight)
	{
		// Arrange
		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceDownsizedImageSize = new ImageSize(downsizedImageWidth, downsizedImageHeight);

		// Act
		var downsizedImageSize = _imageResizeCalculator.GetDownsizedImageSize(PortraitImageSize, viewPortSize);

		// Assert
		Assert.Equal(referenceDownsizedImageSize, downsizedImageSize);
	}

	private static readonly ImageSize LandscapeImageSize = new(1920, 1080);
	private static readonly ImageSize PortraitImageSize = new(1080, 1920);

	private readonly ImageResizeCalculator _imageResizeCalculator;
}
