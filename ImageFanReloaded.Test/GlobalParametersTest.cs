using FluentAssertions;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Global;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded.Test;

public class GlobalParametersTest : TestBase
{
	public GlobalParametersTest()
	{
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(imageResizeCalculator, imageResizer);
	}
	
	[Fact]
	public void GlobalParameters_IsCorrectlyInitialized()
	{
		// Arrange

		// Act

		// Assert
		_globalParameters.ProcessorCount.Should().NotBe(0);
		_globalParameters.ThumbnailSize.Width.Should().NotBe(0);
		_globalParameters.ThumbnailSize.Height.Should().NotBe(0);
		
		_globalParameters.InvalidImage.GetBitmap().Should().NotBeNull();
		_globalParameters.InvalidImageThumbnail.GetBitmap().Should().NotBeNull();
		_globalParameters.LoadingImageThumbnail.GetBitmap().Should().NotBeNull();

		_globalParameters.NameComparer.Should().NotBeNull();
		
		_globalParameters.ImageFileExtensions.Should().NotBeEmpty();
		
		_globalParameters.UserProfilePath.Should().NotBeNull();
		_globalParameters.SpecialFolders.Should().NotBeEmpty();
		
		_globalParameters.PersistentImages.Count.Should().Be(3);

		_globalParameters.DriveIcon.GetBitmap().Should().NotBeNull();
		_globalParameters.FolderIcon.GetBitmap().Should().NotBeNull();

		SaveImageToDisc(
			_globalParameters.InvalidImage.GetBitmap(),
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.InvalidImageThumbnail.GetBitmap(),
			$"{nameof(_globalParameters.InvalidImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.LoadingImageThumbnail.GetBitmap(),
			$"{nameof(_globalParameters.LoadingImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.DriveIcon.GetBitmap(),
			$"{nameof(_globalParameters.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.FolderIcon.GetBitmap(),
			$"{nameof(_globalParameters.FolderIcon)}{OutputFileExtension}");
	}
	
	#region Private

	private readonly GlobalParameters _globalParameters;

	#endregion
}
