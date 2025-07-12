using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;
using ImageFanReloaded.Keyboard;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded.Controls;

public partial class ImageEditWindow : Window, IImageEditView
{
	public ImageEditWindow()
	{
		InitializeComponent();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}

	public IGlobalParameters? GlobalParameters
	{
		get => _globalParameters;
		set
		{
			_globalParameters = value;

			_fileSystemStringComparison = _globalParameters!.NameComparer.ToStringComparison();
		}
	}

	public ISaveFileImageFormatFactory? SaveFileImageFormatFactory { get; set; }
	public ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }

	public ImageFileData? ImageFileData { get; set; }

	public IContentTabItem? ContentTabItem { get; set; }

	public async Task LoadImage()
	{
		try
		{
			_editableImage = await Task.Run(() =>
				new EditableImage(
					ImageFileData!.ImageFilePath, GlobalParameters!.ImageQualityLevel));
		}
		catch
		{
		}

		EnableButtons();

		UpdateDisplayImage();

		Title = IsImageEditingEnabled
			? ImageFileData!.ImageFileName
			: $"Error loading image '{ImageFileData!.ImageFileName}'";
	}

	public event EventHandler<ContentTabItemEventArgs>? ImageChanged;
	public event EventHandler<ContentTabItemEventArgs>? FolderChanged;

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private IGlobalParameters? _globalParameters;
	private StringComparison? _fileSystemStringComparison;

	private EditableImage? _editableImage;

	private bool _hasUnsavedChanges;

	private async void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();
		}
		else if (ShouldExecuteImageEditing(keyModifiers, keyPressing))
		{
			await ExecuteImageEdit(keyPressing);
		}
		else if (ShouldSaveImageAsWithSameFormat(keyModifiers, keyPressing))
		{
			await SaveImageAsWithSameFormat();
		}
		else if (ShouldSaveImageAsWithFormatJpeg(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
		}
		else if (ShouldSaveImageAsWithFormatGif(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
		}
		else if (ShouldSaveImageAsWithFormatPng(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
		}
		else if (ShouldSaveImageAsWithFormatWebp(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
		}
		else if (ShouldSaveImageAsWithFormatTiff(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
		}
		else if (ShouldSaveImageAsWithFormatBmp(keyModifiers, keyPressing))
		{
			await SaveImageAsWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);
		}

		e.Handled = true;
	}

	private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		if (_hasUnsavedChanges)
		{
			e.Cancel = true;

			var closeWindowMessageBox = MessageBoxManager.GetMessageBoxStandard(
				"Unsaved image changes",
				"You have unsaved image changes. Are you sure you want to close the window?",
				MsBox.Avalonia.Enums.ButtonEnum.YesNoCancel,
				MsBox.Avalonia.Enums.Icon.Question,
				WindowStartupLocation.CenterOwner);

			var closeWindowButtonResult = await closeWindowMessageBox.ShowWindowDialogAsync(this);

			if (closeWindowButtonResult != MsBox.Avalonia.Enums.ButtonResult.Yes)
			{
				return;
			}
			else
			{
				_hasUnsavedChanges = false;

				Close();
			}
		}
		else
		{
			_editableImage?.Dispose();
		}
	}

	private async void OnRotateLeft(object? sender, RoutedEventArgs e) => await RotateLeft();
	private async void OnRotateRight(object? sender, RoutedEventArgs e) => await RotateRight();
	private async void OnFlipHorizontally(object? sender, RoutedEventArgs e)
		=> await FlipHorizontally();
	private async void OnFlipVertically(object? sender, RoutedEventArgs e)
		=> await FlipVertically();

	private async void OnSaveImageAsWithSameFormat(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithSameFormat();

	private async void OnSaveImageAsWithFormatJpeg(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatGif(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatPng(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatWebp(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatTiff(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatBmp(object? sender, RoutedEventArgs e)
		=> await SaveImageAsWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);

	private bool IsImageEditingEnabled => _editableImage is not null;

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.EscapeKey ||
			 keyPressing == GlobalParameters!.EnterKey ||
			 keyPressing == GlobalParameters!.DKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldExecuteImageEditing(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.LKey ||
			 keyPressing == GlobalParameters!.RKey ||
			 keyPressing == GlobalParameters!.HKey ||
			 keyPressing == GlobalParameters!.VKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithSameFormat(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatJpeg(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.JKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatGif(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.GKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatPng(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.PKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatWebp(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.WKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatTiff(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.TKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAsWithFormatBmp(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.BKey)
		{
			return true;
		}

		return false;
	}

	private async Task ExecuteImageEdit(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyPressing == GlobalParameters!.LKey)
		{
			await RotateLeft();
		}
		else if (keyPressing == GlobalParameters!.RKey)
		{
			await RotateRight();
		}
		else if (keyPressing == GlobalParameters!.HKey)
		{
			await FlipHorizontally();
		}
		else if (keyPressing == GlobalParameters!.VKey)
		{
			await FlipVertically();
		}
	}

	private async Task RotateLeft()
	{
		if (_editableImage == null)
		{
			return;
		}

		await ApplyTransform(_editableImage.RotateLeft());
	}

	private async Task RotateRight()
	{
		if (_editableImage == null)
		{
			return;
		}

		await ApplyTransform(_editableImage.RotateRight());
	}

	private async Task FlipHorizontally()
	{
		if (_editableImage == null)
		{
			return;
		}

		await ApplyTransform(_editableImage.FlipHorizontally());
	}

	private async Task FlipVertically()
	{
		if (_editableImage == null)
		{
			return;
		}

		await ApplyTransform(_editableImage.FlipVertically());
	}

	private async Task SaveImageAsWithSameFormat() => await SaveImageAs(default);

	private async Task SaveImageAsWithFormat(ISaveFileImageFormat saveFileImageFormat)
		=> await SaveImageAs(saveFileImageFormat);

	private async Task SaveImageAs(ISaveFileImageFormat? saveFileImageFormat)
	{
		if (_editableImage == null)
		{
			return;
		}

		var hasSameFormat = saveFileImageFormat is null;

		var imageFileName = hasSameFormat
			? ImageFileData!.ImageFileName
			: $"{ImageFileData!.ImageFileNameWithoutExtension}{saveFileImageFormat!.Extension}";
		var imageFilePath = ImageFileData!.ImageFilePath;
		var imageFolderPath = ImageFileData!.ImageFolderPath;

		var saveFileDialog = SaveFileDialogFactory!.GetSaveFileDialog();
		var saveFileDialogTitle = hasSameFormat
			? $"Select image file"
			: $"Select {saveFileImageFormat!.Name} image file";
		var imageToSaveFilePath = await saveFileDialog.ShowDialog(
			imageFileName, imageFolderPath, saveFileDialogTitle);

		if (imageToSaveFilePath is not null)
		{
			try
			{
				if (hasSameFormat)
				{
					await _editableImage.SaveImageWithSameFormat(imageToSaveFilePath);
				}
				else
				{
					await _editableImage.SaveImageWithFormat(
						imageToSaveFilePath, saveFileImageFormat!);
				}

				_hasUnsavedChanges = false;

				if (HasOverwrittenCurrentImageFile(imageToSaveFilePath, imageFilePath))
				{
					ImageChanged?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));
				}
				else if (HasSavedImageFileInCurrentFolder(imageToSaveFilePath, imageFolderPath))
				{
					FolderChanged?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));
				}
			}
			catch
			{
				var saveImageAsErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
					"Image file save error",
					$"Could not save image file '{imageToSaveFilePath}'.",
					MsBox.Avalonia.Enums.ButtonEnum.Ok,
					MsBox.Avalonia.Enums.Icon.Error,
					WindowStartupLocation.CenterOwner);

				await saveImageAsErrorMessageBox.ShowWindowDialogAsync(this);
			}
		}
	}

	private async Task ApplyTransform(Task<EditableImage> transformEditableImage)
	{
		try
		{
			var transformedEditableImage = await transformEditableImage;

			_displayImage.Source = transformedEditableImage.ImageToDisplay;

			_editableImage!.Dispose();
			_editableImage = transformedEditableImage;

			UpdateDisplayImage();

			_hasUnsavedChanges = true;
		}
		catch
		{
			var applyTransformErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
				"Image transformation error",
				$"Could not perform image transformation.",
				MsBox.Avalonia.Enums.ButtonEnum.Ok,
				MsBox.Avalonia.Enums.Icon.Error,
				WindowStartupLocation.CenterOwner);

			await applyTransformErrorMessageBox.ShowWindowDialogAsync(this);
		}
	}

	private bool HasOverwrittenCurrentImageFile(string imageToSaveFilePath, string imageFilePath)
		=> imageToSaveFilePath.Equals(imageFilePath, _fileSystemStringComparison!.Value);

	private bool HasSavedImageFileInCurrentFolder(
		string imageToSaveFilePath, string imageFolderPath)
		=> imageToSaveFilePath.StartsWith(imageFolderPath, _fileSystemStringComparison!.Value);

	private void EnableButtons()
	{
		_rotateDropDownButton.IsEnabled = IsImageEditingEnabled;
		_flipDropDownButton.IsEnabled = IsImageEditingEnabled;
		_saveAsDropDownButton.IsEnabled = IsImageEditingEnabled;
	}

	private void UpdateDisplayImage()
	{
		if (IsImageEditingEnabled)
		{
			_displayImage.MaxWidth = _editableImage!.ImageSize.Width;
			_displayImage.MaxHeight = _editableImage!.ImageSize.Height;
			_displayImage.Source = _editableImage!.ImageToDisplay;
		}
	}

	#endregion
}
