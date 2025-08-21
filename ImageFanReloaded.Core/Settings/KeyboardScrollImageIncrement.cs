using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.Settings;

public enum KeyboardScrollImageIncrement
{
	Four = 4,
	Five = 5,
	Six = 6,
	Seven = 7,
	Eight = 8,
	Nine = 9,
	Ten = 10,
	Eleven = 11,
	Twelve = 12,
	Thirteen = 13,
	Fourteen = 14,
	Fifteen = 15,
	Sixteen = 16,
	Seventeen = 17,
	Eighteen = 18,
	Nineteen = 19,
	Twenty = 20,
	TwentyOne = 21,
	TwentyTwo = 22,
	TwentyThree = 23,
	TwentyFour = 24
}

public static class KeyboardScrollImageIncrementExtensions
{
	public static IReadOnlyList<KeyboardScrollImageIncrement> KeyboardScrollImageIncrements
		=> (Enum.GetValues(typeof(KeyboardScrollImageIncrement)) as
				IReadOnlyList<KeyboardScrollImageIncrement>)!;

	public static int ToInt(this KeyboardScrollImageIncrement keyboardScrollImageIncrement)
		=> (int)keyboardScrollImageIncrement;
}
