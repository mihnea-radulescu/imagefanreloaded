using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
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

	public void SetImageInfoText(string text) => _imageInfoTextBox.Text = text;
	
	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private void OnWindowLoaded(object? sender, RoutedEventArgs e) => _imageInfoScrollViewer.Focus();

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

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    (keyPressing == GlobalParameters!.EscapeKey ||
		     keyPressing == GlobalParameters!.EnterKey ||
		     keyPressing == GlobalParameters!.FKey))
		{
			return true;
		}
		
		return false;
	}

	#endregion
}
