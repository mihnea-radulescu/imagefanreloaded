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
	
	public KeyModifiers NoneKeyModifier { get; }
	public KeyModifiers CtrlKeyModifier { get; }
	public KeyModifiers AltKeyModifier { get; }
	public KeyModifiers ShiftKeyModifier { get; }
	
	public Key F4Key { get; }
	
	public Key RKey { get; }
	
	public Key TKey { get; }
	public Key IKey { get; }
	
	public HashSet<Key> FolderTreeNavigationKeys { get; }
	
	public HashSet<Key> ThumbnailsBackwardNavigationKeys { get; }
	public HashSet<Key> ThumbnailsForwardNavigationKeys { get; }
	
	public HashSet<Key> ImagesBackwardNavigationKeys { get; }
	public HashSet<Key> ImagesForwardNavigationKeys { get; }
	
	public StringComparer NameComparer { get; }

	public bool CanDisposeImage(IImage image) => !PersistentImages.Contains(image);
	
	public HashSet<string> ImageFileExtensions { get; }
	
	public string UserProfilePath { get; }
	public IReadOnlyList<string> SpecialFolders { get; }

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
	
	public abstract string AboutTitle { get; }
	public abstract string AboutText { get; }

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

		NoneKeyModifier = KeyModifiers.None;
		CtrlKeyModifier = KeyModifiers.Ctrl;
		AltKeyModifier = KeyModifiers.Alt;
		ShiftKeyModifier = KeyModifiers.Shift;
		
		F4Key = Key.F4;

		RKey = Key.R;

		TKey = Key.T;
		IKey = Key.I;

		FolderTreeNavigationKeys = [
			Key.Up,
			Key.Down,
			Key.Left,
			Key.Right
		];

		ThumbnailsBackwardNavigationKeys = [
			Key.W,
			Key.A
		];

		ThumbnailsForwardNavigationKeys = [
			Key.S,
			Key.D
		];
		
		ImagesBackwardNavigationKeys = [
			Key.Up,
			Key.Left,
			Key.W,
			Key.A
		];

		ImagesForwardNavigationKeys = [
			Key.Down,
			Key.Right,
			Key.S,
			Key.D
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
	}
	
	protected const int IconSizeSquareLength = 36;
	
	#endregion
}
