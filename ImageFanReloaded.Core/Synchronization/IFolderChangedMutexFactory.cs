namespace ImageFanReloaded.Core.Synchronization;

public interface IFolderChangedMutexFactory
{
	IFolderChangedMutex GetFolderChangedMutex();
}
