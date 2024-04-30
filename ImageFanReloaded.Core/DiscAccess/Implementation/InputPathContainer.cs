using System;
using System.IO;
using System.Threading;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathContainer : IInputPathContainer
{
	public InputPathContainer(string? inputPath)
	{
		InputPath = null;
		
		if (!string.IsNullOrEmpty(inputPath) && Path.Exists(inputPath))
		{
			InputPath = Path.GetFullPath(inputPath);
		}

		_readerWriterLockSlim = new ReaderWriterLockSlim();

		_hasPopulatedInputPath = false;
		_hasBeenDisposed = false;
	}
	
	public string? InputPath { get; }

	public bool HasPopulatedInputPath
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();
			
			_readerWriterLockSlim.EnterReadLock();

			var hasPopulatedInputPath = _hasPopulatedInputPath;
			
			_readerWriterLockSlim.ExitReadLock();

			return hasPopulatedInputPath;
		}
		set
		{
			ThrowObjectDisposedExceptionIfNecessary();
			
			_readerWriterLockSlim.EnterWriteLock();
			
			_hasPopulatedInputPath = value;
			
			_readerWriterLockSlim.ExitWriteLock();
		}
	}

	public bool ShouldPopulateInputPath()
	{
		ThrowObjectDisposedExceptionIfNecessary();
		
		var shouldPopulateInputPath = !HasPopulatedInputPath && InputPath is not null;
		return shouldPopulateInputPath;
	}

	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			_readerWriterLockSlim.Dispose();

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
	
	#region Private

	private readonly ReaderWriterLockSlim _readerWriterLockSlim;
	
	private bool _hasPopulatedInputPath;
	private bool _hasBeenDisposed;

	private void ThrowObjectDisposedExceptionIfNecessary()
	{
		if (_hasBeenDisposed)
		{
			throw new ObjectDisposedException(nameof(InputPathContainer));
		}
	}

	#endregion
}
