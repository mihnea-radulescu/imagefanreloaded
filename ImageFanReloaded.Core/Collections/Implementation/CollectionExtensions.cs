using System.Collections.Generic;

namespace ImageFanReloaded.Core.Collections.Implementation;

public static class CollectionExtensions
{
	public static bool IsIndexWithinBounds<T>(this IReadOnlyCollection<T> collection, int index)
		=> index >= 0 && index < collection.Count;
}
