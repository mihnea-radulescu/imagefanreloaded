namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class AsyncAutoResetEventFactory : IAsyncAutoResetEventFactory
{
	public IAsyncAutoResetEvent GetAsyncAutoResetEvent()
		=> new AsyncAutoResetEvent();
}
