using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.Settings.Implementation;

public abstract class GlobalParametersBase : IGlobalParameters
{
	public int ProcessorCount { get; }

	public bool IsLinux { get; }
	public bool IsWindows { get; }
	public bool IsMacOS { get; }

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

	public Key RKey { get; }
	public Key EKey { get; }

	public Key TKey { get; }
	public Key IKey { get; }

	public Key UpKey { get; }
	public Key DownKey { get; }
	public Key LeftKey { get; }
	public Key RightKey { get; }

	public Key PlusKey { get; }
	public Key MinusKey { get; }

	public Key PageUpKey { get; }
	public Key PageDownKey { get; }

	public bool IsBackwardNavigationKey(Key aKey) => _backwardNavigationKeys.Contains(aKey);
	public bool IsForwardNavigationKey(Key aKey) => _forwardNavigationKeys.Contains(aKey);
	public bool IsNavigationKey(Key aKey) => _navigationKeys.Contains(aKey);

	public StringComparer NameComparer { get; }

	public bool CanDisposeImage(IImage image) => !PersistentImages.Contains(image);

	public HashSet<string> DirectlySupportedImageFileExtensions { get; }
	public HashSet<string> IndirectlySupportedImageFileExtensions { get; }
	public HashSet<string> ImageFileExtensions { get; }

	public string UserProfilePath { get; }
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

	#region Protected
	
	protected GlobalParametersBase(IOperatingSystemSettings operatingSystemSettings)
	{
		ProcessorCount = Environment.ProcessorCount;
		
		IsLinux = operatingSystemSettings.IsLinux;
		IsWindows = operatingSystemSettings.IsWindows;
		IsMacOS = operatingSystemSettings.IsMacOS;
		
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

		RKey = Key.R;
		EKey = Key.E;

		TKey = Key.T;
		IKey = Key.I;

		UpKey = Key.Up;
		DownKey = Key.Down;
		LeftKey = Key.Left;
		RightKey = Key.Right;

		PlusKey = Key.Plus;
		MinusKey = Key.Minus;

		PageUpKey = Key.PageUp;
		PageDownKey = Key.PageDown;
		
		NameComparer = operatingSystemSettings.IsWindows
			? new NaturalSortingComparer(StringComparer.InvariantCultureIgnoreCase)
			: new NaturalSortingComparer(StringComparer.InvariantCulture);
		
		DirectlySupportedImageFileExtensions = new HashSet<string>(
			StringComparer.InvariantCultureIgnoreCase)
		{
			".bmp",
			".cr2",
			".cur",
			".dng",
			".gif",
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
			".wbmp",
			".webp"
		};

		IndirectlySupportedImageFileExtensions = new HashSet<string>(
			StringComparer.InvariantCultureIgnoreCase)
		{
			".dds",
			".exr",
			".fts",
			".hdr",
			".heic",
			".heif",
			".jp2",
			".mng",
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

		ImageFileExtensions = new HashSet<string>(
			[.. DirectlySupportedImageFileExtensions,
			 .. IndirectlySupportedImageFileExtensions],
			StringComparer.InvariantCultureIgnoreCase);
		
		UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		SpecialFolders = new List<string>
		{
			"Desktop",
			"Documents",
			"Downloads",
			"Pictures"
		};
		
		_backwardNavigationKeys = [
			Key.Up,
			Key.Left
		];

		_forwardNavigationKeys = [
			Key.Down,
			Key.Right
		];

		_navigationKeys = [ .._backwardNavigationKeys, .._forwardNavigationKeys ];
	}
	
	protected const int IconSizeSquareLength = 36;
	
	#endregion
	
	#region Private

	private readonly HashSet<Key> _backwardNavigationKeys;
	private readonly HashSet<Key> _forwardNavigationKeys;
	private readonly HashSet<Key> _navigationKeys;

	#endregion
}
