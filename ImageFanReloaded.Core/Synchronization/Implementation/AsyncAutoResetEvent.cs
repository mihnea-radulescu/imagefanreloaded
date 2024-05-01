using System.Threading;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class AsyncAutoResetEvent : IAsyncAutoResetEvent
{
	public AsyncAutoResetEvent()
	{
		_autoResetEvent = new AutoResetEvent(true);
	}

	public async Task WaitOne()
		=> await Task.Run(() => _autoResetEvent.WaitOne());

	public async Task Set()
		=> await Task.Run(() => _autoResetEvent.Set());
	
	#region Private

	private readonly AutoResetEvent _autoResetEvent;

	#endregion
}
