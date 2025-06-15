using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Collections.Implementation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core;

public class ImageViewPresenter
{
	public ImageViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IInputPathHandler inputPathHandler,
		IGlobalParameters globalParameters,
		IImageView imageView)
	{
		_discQueryEngine = discQueryEngine;
		_inputPathHandler = inputPathHandler;

		_nameComparison = globalParameters.NameComparer.ToStringComparison();
		
		_imageView = imageView;
		_imageView.ImageChanged += OnImageChanged;
		
		_imageFiles = new List<IImageFile>();
		_currentImageFileIndex = 0;
	}
	
	public async Task SetUpAccessToImages()
	{
		_imageFiles = await _discQueryEngine.GetImageFiles(_inputPathHandler.FolderPath!, false);
			
		(_currentImageFile, _currentImageFileIndex) = _imageFiles
			.Select((anImageFile, index) => (anImageFile, index))
			.Single(anImageFileWithIndex => anImageFileWithIndex.anImageFile.ImageFilePath.Equals(
				_inputPathHandler.FilePath,
				_nameComparison));
		
		LoadCurrentImage();
	}
	
	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IInputPathHandler _inputPathHandler;
	
	private readonly StringComparison _nameComparison;
	
	private readonly IImageView _imageView;

	private IReadOnlyList<IImageFile> _imageFiles;
	private IImageFile? _currentImageFile;
	private int _currentImageFileIndex;

	private void LoadCurrentImage()
	{
		_currentImageFile!.ReadImageDataFromDisc();
		
		_imageView.SetImage(_currentImageFile);
	}

	private void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var newImageFileIndex = _currentImageFileIndex + e.Increment;

		var canAdvanceToNextImage = _imageFiles.IsIndexWithinBounds(newImageFileIndex);
		_imageView.ContinueWithSlideshow = canAdvanceToNextImage;

		if (canAdvanceToNextImage)
		{
			_currentImageFileIndex = newImageFileIndex;
			_currentImageFile = _imageFiles[_currentImageFileIndex];

			LoadCurrentImage();
		}
	}

	#endregion
}
