using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class TabOptionsWindow : Window, ITabOptionsView
{
	public TabOptionsWindow()
	{
		InitializeComponent();
		
		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}
	
	public IGlobalParameters? GlobalParameters { get; set; }
	
	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);
	
	#region Private

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();
		}
		
		e.Handled = true;
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    (keyPressing == GlobalParameters!.EscapeKey ||
		     keyPressing == GlobalParameters!.EnterKey ||
		     keyPressing == GlobalParameters!.OKey))
		{
			return true;
		}
		
		return false;
	}

	#endregion
}
