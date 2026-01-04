using System;
using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class Image : DisposableBase, IImage
{
	public Image(IDisposable imageImplementationInstance, ImageSize imageSize)
	{
		IImageFrame singleImageFrame = new ImageFrame(imageImplementationInstance, imageSize, TimeSpan.Zero);

		_imageFrames = [singleImageFrame];

		_isAnimated = false;
		_totalImageFramesDelay = TimeSpan.Zero;
	}

	public Image(IReadOnlyList<IImageFrame> imageFrames)
	{
		_imageFrames = imageFrames;

		_isAnimated = _imageFrames.Count > 1;
		_totalImageFramesDelay = GetTotalImageFramesDelay();
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

	public bool IsAnimated
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _isAnimated;
		}
	}

	public TimeSpan TotalImageFramesDelay
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _totalImageFramesDelay;
		}
	}

	public IReadOnlyList<IImageFrame> ImageFrames
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageFrames;
		}
	}

	public bool DoesFitWithinViewPort(ImageSize viewPortSize)
		=> Size.Width <= viewPortSize.Width && Size.Height <= viewPortSize.Height;

	public double GetMaxUpscalingFactorToViewPort(ImageSize viewPortSize)
		=> Math.Min(viewPortSize.Width / (double)Size.Width, viewPortSize.Height / (double)Size.Height);

	protected override void DisposeSpecific()
	{
		foreach (var anImageFrame in _imageFrames)
		{
			anImageFrame.Dispose();
		}
	}

	private readonly bool _isAnimated;
	private readonly TimeSpan _totalImageFramesDelay;

	private readonly IReadOnlyList<IImageFrame> _imageFrames;

	private TimeSpan GetTotalImageFramesDelay()
	{
		var imageFramesTotalMilliseconds =
			_imageFrames.Sum(anImageFrame => anImageFrame.DelayUntilNextFrame.TotalMilliseconds);

		var imageFramesTotalTimeSpan = TimeSpan.FromMilliseconds(imageFramesTotalMilliseconds);
		return imageFramesTotalTimeSpan;
	}
}
