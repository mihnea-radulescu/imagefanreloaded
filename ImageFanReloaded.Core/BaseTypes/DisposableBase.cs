using System;

namespace ImageFanReloaded.Core.BaseTypes;

public abstract class DisposableBase
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
	{
		if (_hasBeenDisposed)
		{
			throw new ObjectDisposedException(GetType().Name);
		}
	}

	#endregion

	#region Private

	private bool _hasBeenDisposed;

	#endregion
}
