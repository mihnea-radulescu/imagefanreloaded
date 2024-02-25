using FluentAssertions;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;

namespace ImageFanReloaded.Test;

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
	public void GetResizedImageSize_LandscapeImage_ReturnsCorrectResizedImageSize(
		int viewPortWidth, int viewPortHeight, int resizedImageWidth, int resizedImageHeight)
	{
		// Arrange
		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceResizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		// Act
		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
			LandscapeImageSize, viewPortSize);

		// Assert
		resizedImageSize.Should().Be(referenceResizedImageSize);
	}

	[Theory]
	[InlineData(3840, 2160, 1080, 1920)]
	[InlineData(1920, 1080, 607, 1080)]
	[InlineData(1280, 720, 405, 720)]
	[InlineData(960, 540, 303, 540)]
	public void GetResizedImageSize_PortraitImage_ReturnsCorrectResizedImageSize(
		int viewPortWidth, int viewPortHeight, int resizedImageWidth, int resizedImageHeight)
	{
		// Arrange
		var viewPortSize = new ImageSize(viewPortWidth, viewPortHeight);
		var referenceResizedImageSize = new ImageSize(resizedImageWidth, resizedImageHeight);

		// Act
		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
			PortraitImageSize, viewPortSize);

		// Assert
		resizedImageSize.Should().Be(referenceResizedImageSize);
	}

	#region Private

	private static readonly ImageSize LandscapeImageSize = new ImageSize(1920, 1080);
	private static readonly ImageSize PortraitImageSize = new ImageSize(1080, 1920);

	private readonly ImageResizeCalculator _imageResizeCalculator;

	#endregion
}
