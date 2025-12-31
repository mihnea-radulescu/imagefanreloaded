using System;
using System.Collections.Generic;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Test.TestClasses;

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
		Assert.NotEqual(0, _globalParameters.ProcessorCount);

		List<bool> isOperatingSystemCollection =
		[
			_globalParameters.IsLinux,
			_globalParameters.IsWindows,
			_globalParameters.IsMacOS
		];
		Assert.Single(isOperatingSystemCollection, isOperatingSystem => isOperatingSystem);

		Assert.Equal(6, _globalParameters.MaxRecursionDepth);

		Assert.Equal(KeyModifiers.None, _globalParameters.NoneKeyModifier);

		Assert.NotEqual(KeyModifiers.None, _globalParameters.CtrlKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.AltKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.ShiftKeyModifier);

		Assert.NotEqual(Key.None, _globalParameters.TabKey);
		Assert.NotEqual(Key.None, _globalParameters.EscapeKey);
		Assert.NotEqual(Key.None, _globalParameters.EnterKey);

		Assert.NotEqual(Key.None, _globalParameters.SKey);
		Assert.NotEqual(Key.None, _globalParameters.OKey);

		Assert.NotEqual(Key.None, _globalParameters.HKey);
		Assert.NotEqual(Key.None, _globalParameters.F1Key);

		Assert.NotEqual(Key.None, _globalParameters.FKey);

		Assert.NotEqual(Key.None, _globalParameters.F4Key);

		Assert.NotEqual(Key.None, _globalParameters.NKey);
		Assert.NotEqual(Key.None, _globalParameters.MKey);
		Assert.NotEqual(Key.None, _globalParameters.BKey);

		Assert.NotEqual(Key.None, _globalParameters.AKey);
		Assert.NotEqual(Key.None, _globalParameters.DKey);

		Assert.NotEqual(Key.None, _globalParameters.RKey);
		Assert.NotEqual(Key.None, _globalParameters.GKey);
		Assert.NotEqual(Key.None, _globalParameters.EKey);

		Assert.NotEqual(Key.None, _globalParameters.TKey);
		Assert.NotEqual(Key.None, _globalParameters.IKey);
		Assert.NotEqual(Key.None, _globalParameters.UKey);
		Assert.NotEqual(Key.None, _globalParameters.CKey);

		Assert.NotEqual(Key.None, _globalParameters.Digit1Key);
		Assert.NotEqual(Key.None, _globalParameters.Digit2Key);
		Assert.NotEqual(Key.None, _globalParameters.Digit3Key);

		Assert.NotEqual(Key.None, _globalParameters.UpKey);
		Assert.NotEqual(Key.None, _globalParameters.DownKey);
		Assert.NotEqual(Key.None, _globalParameters.LeftKey);
		Assert.NotEqual(Key.None, _globalParameters.RightKey);
		Assert.NotEqual(Key.None, _globalParameters.BackspaceKey);
		Assert.NotEqual(Key.None, _globalParameters.SpaceKey);

		Assert.NotEqual(Key.None, _globalParameters.PlusKey);
		Assert.NotEqual(Key.None, _globalParameters.MinusKey);

		Assert.NotEqual(Key.None, _globalParameters.PageUpKey);
		Assert.NotEqual(Key.None, _globalParameters.PageDownKey);

		Assert.NotNull(_globalParameters.NameComparer);

		Assert.NotEmpty(_globalParameters.DirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters.IndirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters.AnimationEnabledImageFileExtensions);
		Assert.NotEmpty(_globalParameters.ImageFileExtensions);

		Assert.NotEqual(0U, _globalParameters.ImageQualityLevel);
		Assert.NotEqual(0, _globalParameters.DecimalDigitCountForDisplay);

		Assert.NotNull(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.SpecialFolders);

		Assert.NotNull(_globalParameters.InvalidImage.Bitmap);

		var expectedPersistentImagesCount = 1 + 2 * Enum.GetValues<ThumbnailSize>().Length;
		Assert.Equal(expectedPersistentImagesCount, _globalParameters.PersistentImages.Count);

		Assert.NotNull(_globalParameters.DriveIcon.Bitmap);
		Assert.NotNull(_globalParameters.FolderIcon.Bitmap);

		SaveImageToDisc(
			_globalParameters.InvalidImage.Bitmap,
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.GetInvalidImageThumbnail(ThumbnailSize).Bitmap,
			$"InvalidImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.GetLoadingImageThumbnail(ThumbnailSize).Bitmap,
			$"LoadingImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.DriveIcon.Bitmap,
			$"{nameof(_globalParameters.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.FolderIcon.Bitmap,
			$"{nameof(_globalParameters.FolderIcon)}{OutputFileExtension}");
	}

	#region Private

	private const int ThumbnailSize = 250;

	private readonly GlobalParameters _globalParameters;

	#endregion
}
