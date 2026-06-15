using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum UpsizeFullScreenImageScalingFactor
{
	Disabled = 100,
	To1Dot25X = 125,
	To1Dot50X = 150,
	To1Dot75X = 175,
	To2X = 200,
	To2Dot50X = 250,
	To3X = 300,
	To4X = 400,
	To5X = 500,
	To6X = 600,
	To7X = 700,
	To8X = 800
}

public static class UpsizeFullScreenImageScalingFactorExtensions
{
	public static IReadOnlyList<UpsizeFullScreenImageScalingFactor> Values
		=> Enum.GetValues<UpsizeFullScreenImageScalingFactor>();

	extension(UpsizeFullScreenImageScalingFactor
				upsizeFullScreenImageScalingFactor)
	{
		public double Value
		{
			get
			{
				var value = upsizeFullScreenImageScalingFactor switch
				{
					UpsizeFullScreenImageScalingFactor.Disabled => 1,
					UpsizeFullScreenImageScalingFactor.To1Dot25X => 1.25,
					UpsizeFullScreenImageScalingFactor.To1Dot50X => 1.50,
					UpsizeFullScreenImageScalingFactor.To1Dot75X => 1.75,
					UpsizeFullScreenImageScalingFactor.To2X => 2,
					UpsizeFullScreenImageScalingFactor.To2Dot50X => 2.50,
					UpsizeFullScreenImageScalingFactor.To3X => 3,
					UpsizeFullScreenImageScalingFactor.To4X => 4,
					UpsizeFullScreenImageScalingFactor.To5X => 5,
					UpsizeFullScreenImageScalingFactor.To6X => 6,
					UpsizeFullScreenImageScalingFactor.To7X => 7,
					UpsizeFullScreenImageScalingFactor.To8X => 8,

					_ => throw new NotSupportedException(
							$"Enum value {upsizeFullScreenImageScalingFactor} not supported.")
				};

				return value;
			}
		}

		public string Description
		{
			get
			{
				var description = upsizeFullScreenImageScalingFactor switch
				{
					UpsizeFullScreenImageScalingFactor.Disabled => "Disabled",
					UpsizeFullScreenImageScalingFactor.To1Dot25X => "1.25x",
					UpsizeFullScreenImageScalingFactor.To1Dot50X => "1.50x",
					UpsizeFullScreenImageScalingFactor.To1Dot75X => "1.75x",
					UpsizeFullScreenImageScalingFactor.To2X => "2x",
					UpsizeFullScreenImageScalingFactor.To2Dot50X => "2.50x",
					UpsizeFullScreenImageScalingFactor.To3X => "3x",
					UpsizeFullScreenImageScalingFactor.To4X => "4x",
					UpsizeFullScreenImageScalingFactor.To5X => "5x",
					UpsizeFullScreenImageScalingFactor.To6X => "6x",
					UpsizeFullScreenImageScalingFactor.To7X => "7x",
					UpsizeFullScreenImageScalingFactor.To8X => "8x",

					_ => throw new NotSupportedException(
							$"Enum value {upsizeFullScreenImageScalingFactor} not supported.")
				};

				return description;
			}
		}
	}
}
