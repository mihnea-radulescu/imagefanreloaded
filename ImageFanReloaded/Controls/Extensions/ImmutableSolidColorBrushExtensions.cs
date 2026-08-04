using System.Drawing;
using Avalonia.Media.Immutable;

namespace ImageFanReloaded.Controls.Extensions;

public static class ImmutableSolidColorBrushExtensions
{
	extension(ImmutableSolidColorBrush)
	{
		public static ImmutableSolidColorBrush FromSystemDrawingColor(
			Color color)
				=> new ImmutableSolidColorBrush(
					Avalonia.Media.Color.FromArgb(
						color.A, color.R, color.G, color.B));
	}
}
