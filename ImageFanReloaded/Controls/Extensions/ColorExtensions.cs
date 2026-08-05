using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace ImageFanReloaded.Controls.Extensions;

public static class ColorExtensions
{
	extension(Color color)
	{
		public IBrush GetColorBrush() => new ImmutableSolidColorBrush(color);
	}
}
