namespace ImageFanReloaded.Controls.Extensions;

public static class ColorExtensions
{
	extension(System.Drawing.Color systemColor)
	{
		public Avalonia.Media.Color ToAvaloniaColor()
			=> Avalonia.Media.Color.FromArgb(
				systemColor.A,
				systemColor.R,
				systemColor.G,
				systemColor.B);
	}

	extension(Avalonia.Media.Color avaloniaColor)
	{
		public System.Drawing.Color ToSystemColor()
			=> System.Drawing.Color.FromArgb(
				avaloniaColor.A,
				avaloniaColor.R,
				avaloniaColor.G,
				avaloniaColor.B);

		public Avalonia.Media.IBrush GetColorBrush()
			=> new Avalonia.Media.Immutable.ImmutableSolidColorBrush(
				avaloniaColor);
	}
}
