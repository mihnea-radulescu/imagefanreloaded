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
		_imageView.ImageInfoVisibilityChanged += OnImageInfoVisibilityChanged;
		
		_imageFiles = new List<IImageFile>();
		_currentImageFileIndex = 0;
		
		_showImageInfo = false;
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
	
	private bool _showImageInfo;

	private void LoadCurrentImage()
	{
		_currentImageFile!.ReadImageDataFromDisc();
		
		_imageView.SetImage(_currentImageFile, false, _showImageInfo);
	}
	
	private void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var newImageFileIndex = _currentImageFileIndex + e.Increment;

		if (_imageFiles.IsIndexWithinBounds(newImageFileIndex))
		{
			_currentImageFileIndex = newImageFileIndex;
			_currentImageFile = _imageFiles[_currentImageFileIndex];
			
			LoadCurrentImage();
		}
	}
	
	private void OnImageInfoVisibilityChanged(object? sender, ImageInfoVisibilityChangedEventArgs e)
	{
		_showImageInfo = e.IsVisible;
	}
	
	#endregion
}
