namespace ImageFanReloaded.Core.Synchronization;

public interface IAsyncMutexFactory
{
	IAsyncMutex GetAsyncMutex();
}
