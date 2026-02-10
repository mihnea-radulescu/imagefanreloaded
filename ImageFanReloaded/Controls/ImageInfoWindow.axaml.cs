using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class ImageInfoWindow : Window, IImageInfoView
{
	public ImageInfoWindow()
	{
		InitializeComponent();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}

	public IGlobalParameters? GlobalParameters { get; set; }
	public IImageInfoBuilder? ImageInfoBuilder { get; set; }

	public IImageFile? ImageFile { get; set; }

	public async Task ShowDialog(IMainView owner)
		=> await ShowDialog((Window)owner);

	private bool _isLoading;

	private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
	{
		_isLoading = true;
		SetLoadingImageInfoTitle();

		var imageInfo = await ImageInfoBuilder!.BuildImageInfo(ImageFile!);
		_imageInfoSelectableTextBlock.Text = imageInfo;

		_imageInfoScrollViewer.Focus();

		SetImageInfoTitle();
		_isLoading = false;
	}

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();

			e.Handled = true;
		}
	}

	private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		if (_isLoading)
		{
			e.Cancel = true;
		}
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private void SetLoadingImageInfoTitle()
		=> Title = $"{ImageFile!.ImageFileData.ImageFileName} - loading image info...";

	private void SetImageInfoTitle()
		=> Title = ImageFile!.ImageFileData.ImageFileName;
}
