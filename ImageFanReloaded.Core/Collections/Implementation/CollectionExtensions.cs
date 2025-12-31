using System.Collections.Generic;

namespace ImageFanReloaded.Core.Collections.Implementation;

public static class CollectionExtensions
{
	extension<T>(IReadOnlyCollection<T> collection)
	{
		public bool IsIndexWithinBounds(int index) => index >= 0 && index < collection.Count;
	}
}
