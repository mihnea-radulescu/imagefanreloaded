using System;

namespace ImageFanReloaded.Core.TextHandling.Implementation;

public static class StringExtensions
{
	extension(string longerString)
	{
		public string? GetRemainingStringAfterSubstringMatch(string shorterString, StringComparison stringComparison)
		{
			string? remainingStringAfterSubstringMatch;

			var matchIndex = longerString.IndexOf(shorterString, stringComparison);

			if (matchIndex >= 0)
			{
				var remainingStringStartIndex = matchIndex + shorterString.Length;

				if (remainingStringStartIndex < longerString.Length)
				{
					remainingStringAfterSubstringMatch = longerString[remainingStringStartIndex..];
				}
				else
				{
					remainingStringAfterSubstringMatch = string.Empty;
				}
			}
			else
			{
				remainingStringAfterSubstringMatch = null;
			}

			return remainingStringAfterSubstringMatch;
		}
	}
}
