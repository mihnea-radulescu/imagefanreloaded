using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.Global;

public interface IGlobalParameters
{
	int ProcessorCount { get; }
	ImageSize ThumbnailSize { get; }
	
	bool IsLinux { get; }
	bool IsWindows { get; }
	bool IsMacOS { get; }
	
	Key TabKey { get; }
	Key EscapeKey { get; }
	Key EnterKey { get; }
	Key F1Key { get; }
	
	KeyModifiers AltKeyModifier { get; }
	Key F4Key { get; }
	
	Key RKey { get; }
	
	Key TKey { get; }
	Key IKey { get; }

	HashSet<Key> BackwardNavigationKeys { get; }
	HashSet<Key> ForwardNavigationKeys { get; }
	
	StringComparer NameComparer { get; }

	bool CanDisposeImage(IImage image);
	
	HashSet<string> ImageFileExtensions { get; }
	
	string UserProfilePath { get; }
	IReadOnlyList<string> SpecialFolders { get; }

	IImage InvalidImage { get; }
	IImage InvalidImageThumbnail { get; }
	IImage LoadingImageThumbnail { get; }
	
	HashSet<IImage> PersistentImages { get; }

	IImage DesktopFolderIcon { get; }
	IImage DocumentsFolderIcon { get; }
	IImage DownloadsFolderIcon { get; }
	IImage DriveIcon { get; }
	IImage FolderIcon { get; }
	IImage HomeFolderIcon { get; }
	IImage MediaFolderIcon { get; }
	IImage PicturesFolderIcon { get; }
	
	string AboutTitle { get; }
	string AboutText { get; }
}
