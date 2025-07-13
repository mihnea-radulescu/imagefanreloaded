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

		Title = IsImageLoaded
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
		else if (ShouldRevertChanges(keyModifiers, keyPressing))
		{
			await RevertChanges(keyPressing);
		}
		else if (ShouldEditImage(keyModifiers, keyPressing))
		{
			await EditImage(keyPressing);
		}
		else if (ShouldSaveImageAs(keyModifiers, keyPressing))
		{
			await SaveImageAs(keyPressing);
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

	private async void OnUndo(object? sender, RoutedEventArgs e) => await Undo();
	private async void OnRedo(object? sender, RoutedEventArgs e) => await Redo();

	private async void OnRotateLeft(object? sender, RoutedEventArgs e) => await RotateLeft();
	private async void OnRotateRight(object? sender, RoutedEventArgs e) => await RotateRight();
	private async void OnFlipHorizontally(object? sender, RoutedEventArgs e)
		=> await FlipHorizontally();
	private async void OnFlipVertically(object? sender, RoutedEventArgs e)
		=> await FlipVertically();

	private async void OnSaveImageAsWithSameFormat(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(default);

	private async void OnSaveImageAsWithFormatJpeg(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatGif(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatPng(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatWebp(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatTiff(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatBmp(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);

	private bool IsImageLoaded => _editableImage is not null;

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

	private bool ShouldRevertChanges(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.UKey ||
			 keyPressing == GlobalParameters!.IKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldEditImage(
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

	private bool ShouldSaveImageAs(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.SKey ||
			 keyPressing == GlobalParameters!.JKey ||
			 keyPressing == GlobalParameters!.GKey ||
			 keyPressing == GlobalParameters!.PKey ||
			 keyPressing == GlobalParameters!.WKey ||
			 keyPressing == GlobalParameters!.TKey ||
			 keyPressing == GlobalParameters!.BKey))
		{
			return true;
		}

		return false;
	}

	private async Task RevertChanges(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.UKey)
		{
			await Undo();
		}
		else if (keyPressing == GlobalParameters!.IKey)
		{
			await Redo();
		}
	}

	private async Task EditImage(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

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

	private async Task SaveImageAs(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.SKey)
		{
			await SaveImageWithFormat(default);
		}
		else if (keyPressing == GlobalParameters!.JKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.GKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.PKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.WKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.TKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.BKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);
		}
	}

	private async Task Undo()
	{
		_editableImage!.UndoLastEdit();
		await ApplyTransform(default);
	}

	private async Task Redo()
	{
		_editableImage!.RedoLastEdit();
		await ApplyTransform(default);
	}

	private async Task RotateLeft() => await ApplyTransform(() => _editableImage!.RotateLeft());
	private async Task RotateRight() => await ApplyTransform(() => _editableImage!.RotateRight());

	private async Task FlipHorizontally()
		=> await ApplyTransform(() => _editableImage!.FlipHorizontally());
	private async Task FlipVertically()
		=> await ApplyTransform(() => _editableImage!.FlipVertically());

	private async Task SaveImageWithFormat(ISaveFileImageFormat? saveFileImageFormat)
	{
		if (!IsImageLoaded)
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
					await _editableImage!.SaveImageWithSameFormat(imageToSaveFilePath);
				}
				else
				{
					await _editableImage!.SaveImageWithFormat(
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

	private async Task ApplyTransform(Action? transformImageAction)
	{
		try
		{
			if (transformImageAction is not null)
			{
				await Task.Run(transformImageAction);
			}

			_displayImage.Source = _editableImage!.ImageToDisplay;

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
		_undoButton.IsEnabled = IsImageLoaded;
		_redoButton.IsEnabled = IsImageLoaded;

		_rotateDropDownButton.IsEnabled = IsImageLoaded;
		_flipDropDownButton.IsEnabled = IsImageLoaded;

		_saveAsDropDownButton.IsEnabled = IsImageLoaded;
	}

	private void UpdateDisplayImage()
	{
		if (!IsImageLoaded)
		{
			return;
		}

		_displayImage.MaxWidth = _editableImage!.ImageSize.Width;
		_displayImage.MaxHeight = _editableImage!.ImageSize.Height;
		_displayImage.Source = _editableImage!.ImageToDisplay;

		_undoButton.IsEnabled = _editableImage!.CanUndoLastEdit;
		_redoButton.IsEnabled = _editableImage!.CanRedoLastEdit;

		SizeToContent = SizeToContent.WidthAndHeight;
	}

	#endregion
}
