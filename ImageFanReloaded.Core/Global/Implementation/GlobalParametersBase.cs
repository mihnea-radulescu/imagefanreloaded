using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.Global.Implementation;

public abstract class GlobalParametersBase : IGlobalParameters
{
	public int ProcessorCount { get; }
	public ImageSize ThumbnailSize { get; }
	
	public Key TabKey { get; }
	public Key EscapeKey { get; }
	public Key EnterKey { get; }
	
	public HashSet<Key> BackwardNavigationKeys { get; }
	public HashSet<Key> ForwardNavigationKeys { get; }
	
	public StringComparer NameComparer { get; }

	public bool CanDisposeImage(IImage image) => !PersistentImages.Contains(image);
	
	public HashSet<string> ImageFileExtensions { get; }
	
	public string UserProfilePath { get; }
	public IReadOnlyList<string> SpecialFolders { get; }

	public abstract IImage InvalidImage { get; }
	public abstract IImage InvalidImageThumbnail { get; }
	public abstract IImage LoadingImageThumbnail { get; }
	
	public abstract HashSet<IImage> PersistentImages { get; }
	
	public abstract IImage DriveIcon { get; }
	public abstract IImage FolderIcon { get; }

	#region Protected
	
	protected const int IconSizeSquareLength = 24;
	
	protected GlobalParametersBase()
	{
		ProcessorCount = Environment.ProcessorCount;
		ThumbnailSize = new ImageSize(ThumbnailSizeSquareLength);
		
		TabKey = Key.Tab;
		EscapeKey = Key.Escape;
		EnterKey = Key.Enter;

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
		
		NameComparer = OperatingSystem.IsWindows()
			? new NaturalSortingComparer(StringComparer.InvariantCultureIgnoreCase)
			: new NaturalSortingComparer(StringComparer.InvariantCulture);
		
		ImageFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			".bmp",
			".gif",
			".ico",
			".jpg", ".jpe", ".jpeg",
			".png",
			".tif", ".tiff",
			".webp"
		};
		
		UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		SpecialFolders = new List<string>
		{
			"Desktop",
			"Documents",
			"Downloads",
			"Media",
			"Pictures"
		};
	}
	
	#endregion
	
	#region Private
	
	private const int ThumbnailSizeSquareLength = 250;

	#endregion
}
