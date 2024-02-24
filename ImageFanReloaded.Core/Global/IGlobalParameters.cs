using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.Global;

public interface IGlobalParameters
{
	int ProcessorCount { get; }
	ImageSize ThumbnailSize { get; }
	
	Key TabKey { get; }
	Key EscapeKey { get; }
	Key EnterKey { get; }

	HashSet<Key> BackwardNavigationKeys { get; }
	HashSet<Key> ForwardNavigationKeys { get; }

	bool CanDisposeImage(IImage image);

	IImage InvalidImage { get; }
	IImage InvalidImageThumbnail { get; }
	IImage LoadingImageThumbnail { get; }
	
	HashSet<IImage> PersistentImages { get; }

	IImage DriveIcon { get; }
	IImage FolderIcon { get; }
}
