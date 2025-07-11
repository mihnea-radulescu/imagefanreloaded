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

	public ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }

	public ImageFileData? ImageFileData { get; set; }

	public IContentTabItem? ContentTabItem { get; set; }

	public async Task LoadImage()
	{
		try
		{
			_editableImage = await Task.Run(() =>
				new EditableImage(
					ImageFileData!.ImageFilePath,
					GlobalParameters!.ImageQualityLevel));
		}
		catch
		{
		}

		_rotateLeftButton.IsEnabled = IsImageEditingEnabled;
		_rotateRightButton.IsEnabled = IsImageEditingEnabled;
		_flipHorizontallyButton.IsEnabled = IsImageEditingEnabled;
		_flipVerticallyButton.IsEnabled = IsImageEditingEnabled;
		_saveButton.IsEnabled = IsImageEditingEnabled;

		UpdateDisplayImage();

		Title = IsImageEditingEnabled
			? $"Edit image - {ImageFileData!.ImageFileName}"
			: $"Error loading image - {ImageFileData!.ImageFileName}";
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
			await ExecuteImageEditing(keyPressing);
		}
		else if (ShouldSaveImageAs(keyModifiers, keyPressing))
		{
			await SaveImageAs();
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

	private async void OnSaveAs(object? sender, RoutedEventArgs e) => await SaveImageAs();

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

	private bool ShouldSaveImageAs(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private async Task ExecuteImageEditing(ImageFanReloaded.Core.Keyboard.Key keyPressing)
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

	private async Task SaveImageAs()
	{
		if (_editableImage == null)
		{
			return;
		}

		var imageFileName = ImageFileData!.ImageFileName;
		var imageFilePath = ImageFileData!.ImageFilePath;
		var imageFolderPath = ImageFileData!.ImageFolderPath;

		var saveFileDialog = SaveFileDialogFactory!.GetSaveFileDialog();
		var imageToSaveFilePath = await saveFileDialog.ShowDialog(imageFileName, imageFolderPath);

		if (imageToSaveFilePath is not null)
		{
			try
			{
				await _editableImage.SaveImage(imageToSaveFilePath);

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
				var errorMessageBox = MessageBoxManager.GetMessageBoxStandard(
					"Image File Save Error",
					$"Could not save image file '{imageToSaveFilePath}'.",
					MsBox.Avalonia.Enums.ButtonEnum.Ok,
					MsBox.Avalonia.Enums.Icon.Error,
					WindowStartupLocation.CenterOwner);

				await errorMessageBox.ShowWindowDialogAsync(this);
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
		}
	}

	private bool HasOverwrittenCurrentImageFile(string imageToSaveFilePath, string imageFilePath)
		=> imageToSaveFilePath.Equals(imageFilePath, _fileSystemStringComparison!.Value);

	private bool HasSavedImageFileInCurrentFolder(
		string imageToSaveFilePath, string imageFolderPath)
		=> imageToSaveFilePath.StartsWith(imageFolderPath, _fileSystemStringComparison!.Value);

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
