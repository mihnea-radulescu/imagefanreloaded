using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.Settings;

public interface IGlobalParameters
{
	int ProcessorCount { get; }
	
	bool IsLinux { get; }
	bool IsWindows { get; }
	bool IsMacOS { get; }
	
	KeyModifiers NoneKeyModifier { get; }
	KeyModifiers CtrlKeyModifier { get; }
	KeyModifiers AltKeyModifier { get; }
	KeyModifiers ShiftKeyModifier { get; }

	Key TabKey { get; }
	Key EscapeKey { get; }
	Key EnterKey { get; }
	
	Key F1Key { get; }
	Key HKey { get; }
	Key OKey { get; }
	
	Key F4Key { get; }
	
	Key NKey { get; }
	Key MKey { get; }
	
	Key RKey { get; }
	
	Key TKey { get; }
	Key IKey { get; }
	
	Key UpKey { get; }
	Key DownKey { get; }
	Key LeftKey { get; }
	Key RightKey { get; }
	
	Key PlusKey { get; }
	Key MinusKey { get; }
	
	Key PageUpKey { get; }
	Key PageDownKey { get; }

	bool IsBackwardNavigationKey(Key aKey);
	bool IsForwardNavigationKey(Key aKey);
	bool IsNavigationKey(Key aKey);
	
	StringComparer NameComparer { get; }

	bool CanDisposeImage(IImage image);
	
	HashSet<string> ImageFileExtensions { get; }
	
	string UserProfilePath { get; }
	IReadOnlyList<string> SpecialFolders { get; }

	int ThumbnailSizeIncrement { get; }

	IReadOnlyList<int> GetValidThumbnailSizes();
	bool IsValidThumbnailSize(int thumbnailSize);
	
	IImage InvalidImage { get; }
	HashSet<IImage> PersistentImages { get; }

	IImage DesktopFolderIcon { get; }
	IImage DocumentsFolderIcon { get; }
	IImage DownloadsFolderIcon { get; }
	IImage DriveIcon { get; }
	IImage FolderIcon { get; }
	IImage HomeFolderIcon { get; }
	IImage PicturesFolderIcon { get; }
	
	IImage GetInvalidImageThumbnail(int thumbnailSize);
	IImage GetLoadingImageThumbnail(int thumbnailSize);
}
