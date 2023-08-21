using FluentAssertions;
using Xunit;

namespace ImageFanReloaded.Avalonia.Tests;

public class GlobalDataTest
	: TestBase
{
	[Fact]
	public void GlobalData_IsCorrectlyInitialized()
	{
		// Arange

		// Act

		// Assert
		GlobalData.InvalidImage.Should().NotBeNull();
		GlobalData.InvalidImageThumbnail.Should().NotBeNull();

		GlobalData.LoadingImageThumbnail.Should().NotBeNull();

		GlobalData.DriveIcon.Should().NotBeNull();
		GlobalData.FolderIcon.Should().NotBeNull();

		GlobalData.ProcessorCount.Should().NotBe(0);

		GlobalData.BackwardNavigationKeys.Should().NotBeEmpty();
		GlobalData.ForwardNavigationKeys.Should().NotBeEmpty();

		SaveImageToDisc(
			GlobalData.InvalidImage,
			$"{nameof(GlobalData.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			GlobalData.InvalidImageThumbnail,
			$"{nameof(GlobalData.InvalidImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			GlobalData.LoadingImageThumbnail,
			$"{nameof(GlobalData.LoadingImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			GlobalData.DriveIcon,
			$"{nameof(GlobalData.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			GlobalData.FolderIcon,
			$"{nameof(GlobalData.FolderIcon)}{OutputFileExtension}");
	}
}
