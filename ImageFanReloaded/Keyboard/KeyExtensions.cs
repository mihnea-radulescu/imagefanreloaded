namespace ImageFanReloaded.Keyboard;

public static class KeyExtensions
{
	public static ImageFanReloaded.Core.Keyboard.Key ToCoreKey(this Avalonia.Input.Key key)
		=> key switch
		{
			Avalonia.Input.Key.Tab => Core.Keyboard.Key.Tab,
			Avalonia.Input.Key.Escape => Core.Keyboard.Key.Escape,
			Avalonia.Input.Key.Enter => Core.Keyboard.Key.Enter,

			Avalonia.Input.Key.S => Core.Keyboard.Key.S,
			Avalonia.Input.Key.O => Core.Keyboard.Key.O,

			Avalonia.Input.Key.H => Core.Keyboard.Key.H,
			Avalonia.Input.Key.F1 => Core.Keyboard.Key.F1,

			Avalonia.Input.Key.F => Core.Keyboard.Key.F,

			Avalonia.Input.Key.F4 => Core.Keyboard.Key.F4,

			Avalonia.Input.Key.Up => Core.Keyboard.Key.Up,
			Avalonia.Input.Key.Down => Core.Keyboard.Key.Down,
			Avalonia.Input.Key.Left => Core.Keyboard.Key.Left,
			Avalonia.Input.Key.Right => Core.Keyboard.Key.Right,
			Avalonia.Input.Key.Back => Core.Keyboard.Key.Backspace,
			Avalonia.Input.Key.Space => Core.Keyboard.Key.Space,

			Avalonia.Input.Key.N => Core.Keyboard.Key.N,
			Avalonia.Input.Key.M => Core.Keyboard.Key.M,

			Avalonia.Input.Key.A => Core.Keyboard.Key.A,
			Avalonia.Input.Key.D => Core.Keyboard.Key.D,

			Avalonia.Input.Key.R => Core.Keyboard.Key.R,
			Avalonia.Input.Key.E => Core.Keyboard.Key.E,

			Avalonia.Input.Key.T => Core.Keyboard.Key.T,
			Avalonia.Input.Key.I => Core.Keyboard.Key.I,
			Avalonia.Input.Key.U => Core.Keyboard.Key.U,
			Avalonia.Input.Key.C => Core.Keyboard.Key.C,

			Avalonia.Input.Key.D1 => Core.Keyboard.Key.Digit1,
			Avalonia.Input.Key.D2 => Core.Keyboard.Key.Digit2,
			Avalonia.Input.Key.D3 => Core.Keyboard.Key.Digit3,

			Avalonia.Input.Key.OemPlus => Core.Keyboard.Key.Plus,
			Avalonia.Input.Key.Add => Core.Keyboard.Key.Plus,
			Avalonia.Input.Key.OemMinus => Core.Keyboard.Key.Minus,
			Avalonia.Input.Key.Subtract => Core.Keyboard.Key.Minus,

			Avalonia.Input.Key.PageUp => Core.Keyboard.Key.PageUp,
			Avalonia.Input.Key.PageDown => Core.Keyboard.Key.PageDown,

			_ => Core.Keyboard.Key.None
		};
}
