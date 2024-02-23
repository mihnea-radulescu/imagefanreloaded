using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.Global;

public interface IGlobalParameters
{
	int ProcessorCount { get; }
	ImageSize ThumbnailSize { get; }

	bool CanDisposeImage(IImage image);

	IImage InvalidImage { get; }
	IImage InvalidImageThumbnail { get; }
	IImage LoadingImageThumbnail { get; }
	
	HashSet<IImage> PersistentImages { get; }

	IImage DriveIcon { get; }
	IImage FolderIcon { get; }

	IKeyboardKey TabKey { get; }
	IKeyboardKey EscapeKey { get; }
	IKeyboardKey EnterKey { get; }

	IReadOnlyList<IKeyboardKey> BackwardNavigationKeys { get; }
	IReadOnlyList<IKeyboardKey> ForwardNavigationKeys { get; }
}
