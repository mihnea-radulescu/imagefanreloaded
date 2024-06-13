namespace ImageFanReloaded.Keyboard;

public static class KeyExtensions
{
	public static ImageFanReloaded.Core.Keyboard.Key ToCoreKey(this Avalonia.Input.Key key)
		=> key switch
		{
			Avalonia.Input.Key.Tab => Core.Keyboard.Key.Tab,
			Avalonia.Input.Key.Escape => Core.Keyboard.Key.Escape,
			Avalonia.Input.Key.Enter => Core.Keyboard.Key.Enter,
			Avalonia.Input.Key.F1 => Core.Keyboard.Key.F1,
			
			Avalonia.Input.Key.F4 => Core.Keyboard.Key.F4,

			Avalonia.Input.Key.W => Core.Keyboard.Key.W,
			Avalonia.Input.Key.A => Core.Keyboard.Key.A,
			Avalonia.Input.Key.Up => Core.Keyboard.Key.Up,
			Avalonia.Input.Key.Left => Core.Keyboard.Key.Left,
			Avalonia.Input.Key.Back => Core.Keyboard.Key.Backspace,
			Avalonia.Input.Key.PageUp => Core.Keyboard.Key.PageUp,

			Avalonia.Input.Key.S => Core.Keyboard.Key.S,
			Avalonia.Input.Key.D => Core.Keyboard.Key.D,
			Avalonia.Input.Key.Down => Core.Keyboard.Key.Down,
			Avalonia.Input.Key.Right => Core.Keyboard.Key.Right,
			Avalonia.Input.Key.Space => Core.Keyboard.Key.Space,
			Avalonia.Input.Key.PageDown => Core.Keyboard.Key.PageDown,
			
			Avalonia.Input.Key.R => Core.Keyboard.Key.R,
			
			Avalonia.Input.Key.T => Core.Keyboard.Key.T,
			Avalonia.Input.Key.I => Core.Keyboard.Key.I,

			_ => Core.Keyboard.Key.None
		};
}
