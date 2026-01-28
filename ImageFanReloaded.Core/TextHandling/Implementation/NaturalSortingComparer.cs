using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageFanReloaded.Core.TextHandling.Implementation;

public class NaturalSortingComparer : StringComparer, IStringComparisonEnabled
{
	public NaturalSortingComparer(StringComparer defaultStringComparer)
	{
		_defaultStringComparer = defaultStringComparer;
	}

	public override int Compare(string? x, string? y)
	{
		if (x is null && y is null)
		{
			return 0;
		}

		if (x is null)
		{
			return -1;
		}

		if (y is null)
		{
			return 1;
		}

		if (!ContainsDigits(x) || !ContainsDigits(y))
		{
			return _defaultStringComparer.Compare(x, y);
		}

		var maximumContiguousDigitBlockLengthX =
			GetMaximumContiguousDigitBlockLength(x);
		var maximumContiguousDigitBlockLengthY =
			GetMaximumContiguousDigitBlockLength(y);

		var maximumContiguousDigitBlockLength = Math.Max(
			maximumContiguousDigitBlockLengthX,
			maximumContiguousDigitBlockLengthY);

		var paddedX = PadDigitBlocks(x, maximumContiguousDigitBlockLength);
		var paddedY = PadDigitBlocks(y, maximumContiguousDigitBlockLength);

		return _defaultStringComparer.Compare(paddedX, paddedY);
	}

	public override bool Equals(string? x, string? y) => Compare(x, y) == 0;

	public override int GetHashCode(string obj)
	{
		if (!ContainsDigits(obj))
		{
			return _defaultStringComparer.GetHashCode(obj);
		}

		var maximumContiguousDigitBlockLengthObj =
			GetMaximumContiguousDigitBlockLength(obj);
		var paddedObj = PadDigitBlocks(
			obj, maximumContiguousDigitBlockLengthObj);

		return _defaultStringComparer.GetHashCode(paddedObj);
	}

	public StringComparison GetStringComparison()
	{
		var stringComparison =
			_defaultStringComparer == InvariantCultureIgnoreCase
				? StringComparison.InvariantCultureIgnoreCase
				: StringComparison.InvariantCulture;

		return stringComparison;
	}

	private static readonly Regex ContiguousDigitBlockRegex =
		new Regex(@"\d+", RegexOptions.Compiled);

	private readonly StringComparer _defaultStringComparer;

	private static bool ContainsDigits(string text)
		=> ContiguousDigitBlockRegex.IsMatch(text);

	private static int GetMaximumContiguousDigitBlockLength(string text)
	{
		var maximumContiguousDigitBlockLength = ContiguousDigitBlockRegex
			.Matches(text)
			.Select(aMatch => aMatch.Value.Length)
			.Max();

		return maximumContiguousDigitBlockLength;
	}

	private static string PadDigitBlocks(string text, int digitBlockLength)
	{
		var paddedText = ContiguousDigitBlockRegex.Replace(
			text, aMatch => aMatch.Value.PadLeft(digitBlockLength, '0'));

		return paddedText;
	}
}
