using System.Threading;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class FolderChangedEventHandle : IFolderChangedEventHandle
{
	public FolderChangedEventHandle()
	{
		_eventWaitHandle = new AutoResetEvent(true);
	}

	public async Task WaitOne() => await Task.Run(() => _eventWaitHandle.WaitOne());

	public async Task Set() => await Task.Run(() => _eventWaitHandle.Set());
	
	#region Private

	private readonly EventWaitHandle _eventWaitHandle;

	#endregion
}
