using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.AboutInformation;
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
	
	public Key TabKey { get; }
	public Key EscapeKey { get; }
	public Key EnterKey { get; }
	public Key F1Key { get; }
	
	public KeyModifiers AltKeyModifier { get; }
	public KeyModifiers ShiftKeyModifier { get; }
	
	public Key F4Key { get; }
	
	public Key RKey { get; }
	
	public Key TKey { get; }
	public Key IKey { get; }
	
	public HashSet<Key> BackwardNavigationKeys { get; }
	public HashSet<Key> ForwardNavigationKeys { get; }
	
	public StringComparer NameComparer { get; }

	public bool CanDisposeImage(IImage image) => !PersistentImages.Contains(image);
	
	public HashSet<string> ImageFileExtensions { get; }
	
	public string UserProfilePath { get; }
	public IReadOnlyList<string> SpecialFolders { get; }
	
	public string AboutTitle { get; }
	public string AboutText { get; }

	public abstract IImage InvalidImage { get; }
	
	public abstract ImageSize ThumbnailSize { get; }
	public abstract IImage InvalidImageThumbnail { get; }
	public abstract IImage LoadingImageThumbnail { get; }
	
	public abstract HashSet<IImage> PersistentImages { get; }
	
	public abstract IImage DesktopFolderIcon { get; }
	public abstract IImage DocumentsFolderIcon { get; }
	public abstract IImage DownloadsFolderIcon { get; }
	public abstract IImage DriveIcon { get; }
	public abstract IImage FolderIcon { get; }
	public abstract IImage HomeFolderIcon { get; }
	public abstract IImage PicturesFolderIcon { get; }

	#region Protected
	
	protected GlobalParametersBase(
		IOperatingSystemSettings operatingSystemSettings,
		IAboutInformationProvider aboutInformationProvider)
	{
		ProcessorCount = Environment.ProcessorCount;
		
		IsLinux = operatingSystemSettings.IsLinux;
		IsWindows = operatingSystemSettings.IsWindows;
		IsMacOS = operatingSystemSettings.IsMacOS;
		
		TabKey = Key.Tab;
		EscapeKey = Key.Escape;
		EnterKey = Key.Enter;
		F1Key = Key.F1;

		AltKeyModifier = KeyModifiers.Alt;
		ShiftKeyModifier = KeyModifiers.Shift;
		
		F4Key = Key.F4;

		RKey = Key.R;

		TKey = Key.T;
		IKey = Key.I;

		BackwardNavigationKeys = [
			Key.W,
			Key.A,
			Key.Up,
			Key.Left,
			Key.Backspace,
			Key.PageUp
		];

		ForwardNavigationKeys = [
			Key.S,
			Key.D,
			Key.Down,
			Key.Right,
			Key.Space,
			Key.PageDown
		];
		
		NameComparer = operatingSystemSettings.IsWindows
			? new NaturalSortingComparer(StringComparer.InvariantCultureIgnoreCase)
			: new NaturalSortingComparer(StringComparer.InvariantCulture);
		
		ImageFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
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
		
		UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		SpecialFolders = new List<string>
		{
			"Desktop",
			"Documents",
			"Downloads",
			"Pictures"
		};
		
		AboutTitle = "About ImageFan Reloaded";
		
		AboutText =
@$"Cross-platform, light-weight, tab-based image viewer, supporting multi-core processing

Version {aboutInformationProvider.VersionString}
Copyright © Mihnea Rădulescu 2017 - {aboutInformationProvider.Year}

https://github.com/mihnea-radulescu/imagefanreloaded

User interface:

• left mouse button for interacting with tabs and folders, and for selecting, opening, zooming in and out, and dragging images
• right mouse button for returning from the opened image to the main view
• mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through opened images
• key Tab for cycling through active tabs
• key R for toggling recursive folder access, and key combo Shift-R for toggling persistent recursive folder access
• keys W-A-S-D, Up-Down-Left-Right, Backspace-Space and PageUp-PageDown for back and forward navigation through thumbnails and opened images
• key Enter for entering image view and zoomed image view modes
• key I for toggling image info in image view and zoomed image view modes
• keys Esc and T for exiting image view and zoomed image view modes
• key Esc for quitting application
• key F1 for displaying About view";
	}
	
	protected const int IconSizeSquareLength = 36;
	
	#endregion
}