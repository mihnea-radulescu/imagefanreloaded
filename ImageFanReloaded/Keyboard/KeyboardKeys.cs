using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using ImageFanReloaded.Core.Global;

namespace ImageFanReloaded.Keyboard;

public class KeyboardKeys
{
	public KeyboardKeys(IGlobalParameters globalParameters)
	{
		TabKey = globalParameters.TabKey.GetKey();
		EscapeKey = globalParameters.EscapeKey.GetKey();
		EnterKey = globalParameters.EnterKey.GetKey();
		
		BackwardNavigationKeys = globalParameters.BackwardNavigationKeys
			.Select(aNavigationKey => aNavigationKey.GetKey())
			.ToHashSet();
		
		ForwardNavigationKeys = globalParameters.ForwardNavigationKeys
			.Select(aNavigationKey => aNavigationKey.GetKey())
			.ToHashSet();
	}
	
	public Key TabKey { get; }
	public Key EscapeKey { get; }
	public Key EnterKey { get; }

	public HashSet<Key> BackwardNavigationKeys { get; }
	public HashSet<Key> ForwardNavigationKeys { get; }
}
