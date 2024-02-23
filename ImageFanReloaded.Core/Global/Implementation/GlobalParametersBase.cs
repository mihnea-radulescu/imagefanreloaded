using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.Global.Implementation;

public abstract class GlobalParametersBase : IGlobalParameters
{
	protected GlobalParametersBase(
		IImageResizeCalculator imageResizeCalculator,
		IImageResizer imageResizer)
	{
		ImageResizeCalculator = imageResizeCalculator;
		ImageResizer = imageResizer;

		ProcessorCount = Environment.ProcessorCount;
		ThumbnailSize = new ImageSize(ThumbnailSizeSquareLength);
	}
	
	public int ProcessorCount { get; }
	public ImageSize ThumbnailSize { get; }

	public bool CanDisposeImage(IImage image)
		=> !PersistentImages.Contains(image);

	public abstract IImage InvalidImage { get; }
	public abstract IImage InvalidImageThumbnail { get; }
	public abstract IImage LoadingImageThumbnail { get; }
	
	public abstract HashSet<IImage> PersistentImages { get; }
	
	public abstract IImage DriveIcon { get; }
	public abstract IImage FolderIcon { get; }

	public abstract IKeyboardKey TabKey { get; }
	public abstract IKeyboardKey EscapeKey { get; }
	public abstract IKeyboardKey EnterKey { get; }
	
	public abstract IReadOnlyList<IKeyboardKey> BackwardNavigationKeys { get; }
	public abstract IReadOnlyList<IKeyboardKey> ForwardNavigationKeys { get; }
	
	#region Protected
	
	protected const int IconSizeSquareLength = 24;
	
	protected readonly IImageResizeCalculator ImageResizeCalculator;
	protected readonly IImageResizer ImageResizer;
	
	#endregion
	
	#region Private
	
	private const int ThumbnailSizeSquareLength = 250;

	#endregion
}
