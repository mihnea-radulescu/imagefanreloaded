using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class Image : DisposableBase, IImage
{
	public Image(IDisposable imageImplementationInstance, ImageSize imageSize)
	{
		IImageFrame singleImageFrame = new ImageFrame(
			imageImplementationInstance, imageSize, TimeSpan.Zero);

		_imageFrames = new List<IImageFrame> { singleImageFrame };
	}

	public Image(IReadOnlyList<IImageFrame> imageFrames)
	{
		_imageFrames = imageFrames;
	}

	public ImageSize Size
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageFrames[0].Size;
		}
	}

	public TimeSpan DelayUntilNextFrame
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return TimeSpan.Zero;
		}
	}

	public TImageImplementation GetInstance<TImageImplementation>()
		where TImageImplementation : class, IDisposable
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return _imageFrames[0].GetInstance<TImageImplementation>();
	}

	public bool IsAnimated => _imageFrames.Count > 1;

	public IReadOnlyList<IImageFrame> GetImageFrames() => _imageFrames;

	#region Protected

	protected override void DisposeSpecific()
	{
		foreach (var anImageFrame in _imageFrames)
		{
			anImageFrame.Dispose();
		}
	}

	#endregion

	#region Private

	private readonly IReadOnlyList<IImageFrame> _imageFrames;

	#endregion
}
