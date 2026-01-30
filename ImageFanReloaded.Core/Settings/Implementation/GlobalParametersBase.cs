using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.RuntimeEnvironment.Implementation.Extensions;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.Settings.Implementation;

public abstract class GlobalParametersBase : IGlobalParameters
{
	public string ApplicationName => "ImageFanReloaded";

	public int ProcessorCount { get; }

	public RuntimeEnvironmentType RuntimeEnvironmentType { get; }

	public int MaxRecursionDepth => 6;

	public KeyModifiers NoneKeyModifier { get; }
	public KeyModifiers CtrlKeyModifier { get; }
	public KeyModifiers AltKeyModifier { get; }
	public KeyModifiers ShiftKeyModifier { get; }

	public Key TabKey { get; }
	public Key EscapeKey { get; }
	public Key EnterKey { get; }

	public Key SKey { get; }
	public Key OKey { get; }

	public Key HKey { get; }
	public Key F1Key { get; }

	public Key FKey { get; }

	public Key F4Key { get; }

	public Key NKey { get; }
	public Key MKey { get; }
	public Key BKey { get; }

	public Key AKey { get; }
	public Key DKey { get; }

	public Key RKey { get; }
	public Key GKey { get; }
	public Key EKey { get; }

	public Key TKey { get; }
	public Key IKey { get; }
	public Key UKey { get; }
	public Key CKey { get; }

	public Key Digit1Key { get; }
	public Key Digit2Key { get; }
	public Key Digit3Key { get; }
	public Key Digit4Key { get; }

	public Key UpKey { get; }
	public Key DownKey { get; }
	public Key LeftKey { get; }
	public Key RightKey { get; }
	public Key BackspaceKey { get; }
	public Key SpaceKey { get; }

	public Key PlusKey { get; }
	public Key MinusKey { get; }

	public Key PageUpKey { get; }
	public Key PageDownKey { get; }

	public bool IsBackwardNavigationKey(Key aKey)
		=> _backwardNavigationKeys.Contains(aKey);
	public bool IsForwardNavigationKey(Key aKey)
		=> _forwardNavigationKeys.Contains(aKey);
	public bool IsNavigationKey(Key aKey) => _navigationKeys.Contains(aKey);

	public StringComparer NameComparer { get; }

	public bool CanDisposeImage(IImage image)
		=> !PersistentImages.Contains(image);

	public HashSet<string> DirectlySupportedImageFileExtensions { get; }
	public HashSet<string> IndirectlySupportedImageFileExtensions { get; }
	public HashSet<string> AnimationEnabledImageFileExtensions { get; }
	public HashSet<string> ImageFileExtensions { get; }

	public uint ImageQualityLevel { get; }
	public int DecimalDigitCountForDisplay { get; }

	public string UserHomePath { get; }
	public string UserConfigPath { get; }

	public IReadOnlyList<string> SpecialFolders { get; }

	public abstract IImage InvalidImage { get; }
	public abstract HashSet<IImage> PersistentImages { get; }

	public abstract IImage DesktopFolderIcon { get; }
	public abstract IImage DocumentsFolderIcon { get; }
	public abstract IImage DownloadsFolderIcon { get; }
	public abstract IImage DriveIcon { get; }
	public abstract IImage FolderIcon { get; }
	public abstract IImage HomeFolderIcon { get; }
	public abstract IImage PicturesFolderIcon { get; }

	public abstract IImage GetInvalidImageThumbnail(int thumbnailSize);
	public abstract IImage GetLoadingImageThumbnail(int thumbnailSize);

	protected GlobalParametersBase(
		IRuntimeEnvironmentSettings runtimeEnvironmentSettings)
	{
		ProcessorCount = Environment.ProcessorCount;

		RuntimeEnvironmentType = runtimeEnvironmentSettings
			.RuntimeEnvironmentType;

		TabKey = Key.Tab;
		EscapeKey = Key.Escape;
		EnterKey = Key.Enter;

		SKey = Key.S;
		OKey = Key.O;

		HKey = Key.H;
		F1Key = Key.F1;

		FKey = Key.F;

		NoneKeyModifier = KeyModifiers.None;
		CtrlKeyModifier = KeyModifiers.Ctrl;
		AltKeyModifier = KeyModifiers.Alt;
		ShiftKeyModifier = KeyModifiers.Shift;

		F4Key = Key.F4;

		NKey = Key.N;
		MKey = Key.M;
		BKey = Key.B;

		AKey = Key.A;
		DKey = Key.D;

		RKey = Key.R;
		GKey = Key.G;
		EKey = Key.E;

		TKey = Key.T;
		IKey = Key.I;
		UKey = Key.U;
		CKey = Key.C;

		Digit1Key = Key.Digit1;
		Digit2Key = Key.Digit2;
		Digit3Key = Key.Digit3;
		Digit4Key = Key.Digit4;

		UpKey = Key.Up;
		DownKey = Key.Down;
		LeftKey = Key.Left;
		RightKey = Key.Right;
		BackspaceKey = Key.Backspace;
		SpaceKey = Key.Space;

		PlusKey = Key.Plus;
		MinusKey = Key.Minus;

		PageUpKey = Key.PageUp;
		PageDownKey = Key.PageDown;

		var stringComparer = runtimeEnvironmentSettings
			.RuntimeEnvironmentType
			.GetStringComparer();
		NameComparer = new NaturalSortingComparer(stringComparer);

		DirectlySupportedImageFileExtensions =
			new HashSet<string>(ExtensionsStringComparer)
		{
			".bmp",
			".cr2",
			".cur",
			".dng",
			".ico",
			".jfif",
			".jpe", ".jpeg", ".jpg",
			".jps",
			".nef",
			".nrw",
			".pef",
			".png",
			".raf",
			".rw2",
			".wbmp"
		};

		IndirectlySupportedImageFileExtensions = new HashSet<string>(
			ExtensionsStringComparer)
		{
			".dds",
			".exr",
			".fts",
			".hdr",
			".heic",
			".heif",
			".jp2",
			".orf",
			".pam",
			".pbm",
			".pcd",
			".pcx",
			".pes",
			".pfm",
			".pgm",
			".picon",
			".pict",
			".ppm",
			".psd",
			".qoi",
			".sgi",
			".svg",
			".tga",
			".tif", ".tiff",
			".xbm",
			".xpm"
		};

		AnimationEnabledImageFileExtensions = new HashSet<string>(
			ExtensionsStringComparer)
		{
			".gif",
			".mng",
			".webp"
		};

		ImageFileExtensions = new HashSet<string>(
			[..DirectlySupportedImageFileExtensions, 
			 ..IndirectlySupportedImageFileExtensions,
			 ..AnimationEnabledImageFileExtensions],
			ExtensionsStringComparer);

		ImageQualityLevel = 80;
		DecimalDigitCountForDisplay = 2;

		UserHomePath = Environment.GetFolderPath(
			Environment.SpecialFolder.UserProfile);
		UserConfigPath = Environment.GetFolderPath(
			Environment.SpecialFolder.ApplicationData);

		SpecialFolders =
		[
			"Desktop",
			"Documents",
			"Downloads",
			"Pictures"
		];

		_backwardNavigationKeys = [Key.Up, Key.Left, Key.Backspace];

		_forwardNavigationKeys = [Key.Down, Key.Right, Key.Space];

		_navigationKeys = [.._backwardNavigationKeys, .._forwardNavigationKeys];
	}

	protected const int IconSizeSquareLength = 36;

	private static readonly StringComparer ExtensionsStringComparer =
		StringComparer.InvariantCultureIgnoreCase;

	private readonly HashSet<Key> _backwardNavigationKeys;
	private readonly HashSet<Key> _forwardNavigationKeys;
	private readonly HashSet<Key> _navigationKeys;
}
