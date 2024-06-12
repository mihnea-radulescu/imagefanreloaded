using System.Threading;

namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class MutexSlim : SemaphoreSlim
{
	public MutexSlim()
		: base(1, 1)
	{
	}
}
