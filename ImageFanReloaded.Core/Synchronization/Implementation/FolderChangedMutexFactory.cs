namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class FolderChangedMutexFactory : IFolderChangedMutexFactory
{
	public IFolderChangedMutex GetFolderChangedMutex() => new FolderChangedMutex();
}
