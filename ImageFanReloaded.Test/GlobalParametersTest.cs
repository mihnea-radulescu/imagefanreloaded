using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Test;

public class GlobalParametersTest : TestBase
{
	public GlobalParametersTest()
	{
		IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();
		
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(operatingSystemSettings, imageResizer);
	}
	
	[Fact]
	public void GlobalParameters_IsCorrectlyInitialized()
	{
		// Arrange

		// Act

		// Assert
		_globalParameters.ProcessorCount.Should().NotBe(0);

		List<bool> isOperatingSystemCollection =
		[
			_globalParameters.IsLinux,
			_globalParameters.IsWindows,
			_globalParameters.IsMacOS
		];
		isOperatingSystemCollection.Should().ContainSingle(isOperatingSystem => isOperatingSystem == true);
		
		_globalParameters.NoneKeyModifier.Should().Be(KeyModifiers.None);
		_globalParameters.CtrlKeyModifier.Should().NotBe(KeyModifiers.None);
		_globalParameters.AltKeyModifier.Should().NotBe(KeyModifiers.None);
		_globalParameters.ShiftKeyModifier.Should().NotBe(KeyModifiers.None);
		
		_globalParameters.TabKey.Should().NotBe(Key.None);
		_globalParameters.EscapeKey.Should().NotBe(Key.None);
		_globalParameters.EnterKey.Should().NotBe(Key.None);
		
		_globalParameters.F1Key.Should().NotBe(Key.None);
		_globalParameters.HKey.Should().NotBe(Key.None);
		_globalParameters.OKey.Should().NotBe(Key.None);

		_globalParameters.F4Key.Should().NotBe(Key.None);
		
		_globalParameters.NKey.Should().NotBe(Key.None);
		_globalParameters.CKey.Should().NotBe(Key.None);
		_globalParameters.MKey.Should().NotBe(Key.None);
		
		_globalParameters.RKey.Should().NotBe(Key.None);
		
		_globalParameters.TKey.Should().NotBe(Key.None);
		_globalParameters.IKey.Should().NotBe(Key.None);
		
		_globalParameters.UpKey.Should().NotBe(Key.None);
		_globalParameters.DownKey.Should().NotBe(Key.None);
		_globalParameters.LeftKey.Should().NotBe(Key.None);
		_globalParameters.RightKey.Should().NotBe(Key.None);
		
		_globalParameters.PlusKey.Should().NotBe(Key.None);
		_globalParameters.MinusKey.Should().NotBe(Key.None);
		
		_globalParameters.PageUpKey.Should().NotBe(Key.None);
		_globalParameters.PageDownKey.Should().NotBe(Key.None);
		
		_globalParameters.NameComparer.Should().NotBeNull();
		
		_globalParameters.ImageFileExtensions.Should().NotBeEmpty();
		
		_globalParameters.UserProfilePath.Should().NotBeNullOrEmpty();
		_globalParameters.SpecialFolders.Should().NotBeEmpty();
		_globalParameters.DefaultFileSystemEntryInfoOrdering.Should().Be(FileSystemEntryInfoOrdering.NameAscending);

		_globalParameters.DefaultThumbnailSize.Should().Be(250);
		_globalParameters.ThumbnailSizeIncrement.Should().Be(50);
		_globalParameters.IsValidThumbnailSize(_globalParameters.DefaultThumbnailSize).Should().BeTrue();
		
		_globalParameters.InvalidImage.GetBitmap().Should().NotBeNull();
		
		_globalParameters.GetInvalidImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap()
			.Should().NotBeNull();
		_globalParameters.GetLoadingImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap()
			.Should().NotBeNull();
		
		_globalParameters.PersistentImages.Count.Should().Be(17);

		_globalParameters.DriveIcon.GetBitmap().Should().NotBeNull();
		_globalParameters.FolderIcon.GetBitmap().Should().NotBeNull();

		SaveImageToDisc(
			_globalParameters.InvalidImage.GetBitmap(),
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.GetInvalidImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap(),
			$"InvalidImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.GetLoadingImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap(),
			$"LoadingImageThumbnail_{OutputFileExtension}");

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
