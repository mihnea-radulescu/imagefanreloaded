using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class FolderChangedMutex : IFolderChangedMutex
{
	public FolderChangedMutex()
	{
		_mutexSlim = new MutexSlim();
	}

	public async Task Wait() => await _mutexSlim.WaitAsync();

	public void Signal() => _mutexSlim.Release();
	
	#region Private

	private readonly MutexSlim _mutexSlim;

	#endregion
}
