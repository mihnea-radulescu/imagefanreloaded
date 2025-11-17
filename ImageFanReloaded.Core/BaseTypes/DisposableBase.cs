using System;

namespace ImageFanReloaded.Core.BaseTypes;

public abstract class DisposableBase : IDisposable
{
	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			DisposeSpecific();

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	#region Protected

	protected abstract void DisposeSpecific();

	protected void ThrowObjectDisposedExceptionIfNecessary()
		=> ObjectDisposedException.ThrowIf(_hasBeenDisposed, this);

	#endregion

	#region Private

	private bool _hasBeenDisposed;

	#endregion
}
