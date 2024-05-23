using FluentAssertions;
using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.AboutInformation.Implementation;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Global;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded.Test;

public class GlobalParametersTest : TestBase
{
	public GlobalParametersTest()
	{
		IAboutInformationProvider aboutInformationProvider = new AboutInformationProvider();
		
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(aboutInformationProvider, imageResizer);
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
		
		_globalParameters.AltKey.Should().NotBe(Key.None);
		_globalParameters.TabKey.Should().NotBe(Key.None);
		_globalParameters.EscapeKey.Should().NotBe(Key.None);
		_globalParameters.EnterKey.Should().NotBe(Key.None);
		_globalParameters.F1Key.Should().NotBe(Key.None);
		_globalParameters.F4Key.Should().NotBe(Key.None);
		
		_globalParameters.AltKeyModifier.Should().NotBe(KeyModifiers.None);
		
		_globalParameters.BackwardNavigationKeys.Should().NotBeEmpty();
		_globalParameters.ForwardNavigationKeys.Should().NotBeEmpty();
		
		_globalParameters.NameComparer.Should().NotBeNull();
		
		_globalParameters.ImageFileExtensions.Should().NotBeEmpty();
		
		_globalParameters.UserProfilePath.Should().NotBeNullOrEmpty();
		_globalParameters.SpecialFolders.Should().NotBeEmpty();
		
		_globalParameters.InvalidImage.GetBitmap().Should().NotBeNull();
		_globalParameters.InvalidImageThumbnail.GetBitmap().Should().NotBeNull();
		_globalParameters.LoadingImageThumbnail.GetBitmap().Should().NotBeNull();
		
		_globalParameters.PersistentImages.Count.Should().Be(3);

		_globalParameters.DriveIcon.GetBitmap().Should().NotBeNull();
		_globalParameters.FolderIcon.GetBitmap().Should().NotBeNull();
		
		_globalParameters.AboutTitle.Should().NotBeNullOrEmpty();
		_globalParameters.AboutText.Should().NotBeNullOrEmpty();

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
