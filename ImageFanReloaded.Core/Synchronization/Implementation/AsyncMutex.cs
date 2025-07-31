using System;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class AsyncMutex : IAsyncMutex
{
	public AsyncMutex()
	{
		_mutexSlim = new MutexSlim();

		_hasBeenDisposed = false;
	}

	public async Task Wait()
	{
		if (!_hasBeenDisposed)
		{
			try
			{
				await _mutexSlim.WaitAsync();
			}
			catch (ObjectDisposedException)
			{
			}
		}
	}

	public void Signal()
	{
		if (!_hasBeenDisposed)
		{
			try
			{
				_mutexSlim.Release();
			}
			catch (ObjectDisposedException)
			{
			}
		}
	}

	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			_mutexSlim.Dispose();

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	#region Private

	private readonly MutexSlim _mutexSlim;

	private volatile bool _hasBeenDisposed;

	#endregion
}
