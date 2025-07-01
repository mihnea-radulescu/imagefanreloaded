namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class AsyncMutexFactory : IAsyncMutexFactory
{
	public IAsyncMutex GetAsyncMutex() => new AsyncMutex();
}
