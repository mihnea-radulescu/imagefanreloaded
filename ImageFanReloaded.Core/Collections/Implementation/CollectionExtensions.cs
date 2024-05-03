using System.Collections.Generic;

namespace ImageFanReloaded.Core.Collections.Implementation;

public static class CollectionExtensions
{
	public static bool IsIndexWithinBounds<T>(this ICollection<T> collection, int index)
		=> 0 <= index && index < collection.Count;
	
	public static bool IsIndexWithinBounds<T>(this IReadOnlyCollection<T> collection, int index)
		=> 0 <= index && index < collection.Count;
}
