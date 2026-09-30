using NSubstitute;
using Xunit;
using ImageFanReloaded.Core.ImageCore.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Test.TestClasses;

public class ImageIndexCalculatorTest
{
	public ImageIndexCalculatorTest()
	{
		_imageIndexCalculator = new ImageIndexCalculator();
	}

	[Theory]
	[InlineData(5, 0, 0, 0)]
	[InlineData(5, 0, 1, 1)]
	[InlineData(5, 1, 0, 1)]
	[InlineData(5, 1, 2, 3)]
	[InlineData(5, 1, 3, 4)]
	[InlineData(5, 2, 3, 4)]
	[InlineData(5, 1, 5, 4)]
	[InlineData(5, 2, 8, 4)]
	public void GetIncrementedImageIndex_LoopImagesIsFalse_IncrementIsNonNegative_ReturnsCorrectIncrementedIndex(
		int imageCount,
		int imageIndex,
		int increment,
		int referenceIncrementedIndex)
	{
		// Arrange
		var tabOptions = Substitute.For<ITabOptions>();
		tabOptions.LoopImages.Returns(false);

		// Act
		var incrementedIndex = _imageIndexCalculator.GetIncrementedImageIndex(
			tabOptions, imageCount, imageIndex, increment);

		// Assert
		Assert.Equal(referenceIncrementedIndex, incrementedIndex);
	}

	[Theory]
	[InlineData(5, 0, -1, 0)]
	[InlineData(5, 1, -1, 0)]
	[InlineData(5, 2, -1, 1)]
	[InlineData(5, 3, -3, 0)]
	[InlineData(5, 1, -3, 0)]
	[InlineData(5, 4, -3, 1)]
	[InlineData(5, 4, -4, 0)]
	[InlineData(5, 4, -8, 0)]
	public void GetIncrementedImageIndex_LoopImagesIsFalse_IncrementIsNegative_ReturnsCorrectIncrementedIndex(
		int imageCount,
		int imageIndex,
		int increment,
		int referenceIncrementedIndex)
	{
		// Arrange
		var tabOptions = Substitute.For<ITabOptions>();
		tabOptions.LoopImages.Returns(false);

		// Act
		var incrementedIndex = _imageIndexCalculator.GetIncrementedImageIndex(
			tabOptions, imageCount, imageIndex, increment);

		// Assert
		Assert.Equal(referenceIncrementedIndex, incrementedIndex);
	}

	[Theory]
	[InlineData(5, 0, 0, 0)]
	[InlineData(5, 0, 1, 1)]
	[InlineData(5, 1, 0, 1)]
	[InlineData(5, 1, 2, 3)]
	[InlineData(5, 1, 3, 4)]
	[InlineData(5, 2, 3, 0)]
	[InlineData(5, 1, 5, 1)]
	[InlineData(5, 2, 8, 0)]
	public void GetIncrementedImageIndex_LoopImagesIsTrue_IncrementIsNonNegative_ReturnsCorrectIncrementedIndex(
		int imageCount,
		int imageIndex,
		int increment,
		int referenceIncrementedIndex)
	{
		// Arrange
		var tabOptions = Substitute.For<ITabOptions>();
		tabOptions.LoopImages.Returns(true);

		// Act
		var incrementedIndex = _imageIndexCalculator.GetIncrementedImageIndex(
			tabOptions, imageCount, imageIndex, increment);

		// Assert
		Assert.Equal(referenceIncrementedIndex, incrementedIndex);
	}

	[Theory]
	[InlineData(5, 0, -1, 4)]
	[InlineData(5, 1, -1, 0)]
	[InlineData(5, 2, -1, 1)]
	[InlineData(5, 3, -3, 0)]
	[InlineData(5, 1, -3, 3)]
	[InlineData(5, 4, -3, 1)]
	[InlineData(5, 4, -4, 0)]
	[InlineData(5, 4, -8, 1)]
	public void GetIncrementedImageIndex_LoopImagesIsTrue_IncrementIsNegative_ReturnsCorrectIncrementedIndex(
		int imageCount,
		int imageIndex,
		int increment,
		int referenceIncrementedIndex)
	{
		// Arrange
		var tabOptions = Substitute.For<ITabOptions>();
		tabOptions.LoopImages.Returns(true);

		// Act
		var incrementedIndex = _imageIndexCalculator.GetIncrementedImageIndex(
			tabOptions, imageCount, imageIndex, increment);

		// Assert
		Assert.Equal(referenceIncrementedIndex, incrementedIndex);
	}

	private readonly ImageIndexCalculator _imageIndexCalculator;
}
