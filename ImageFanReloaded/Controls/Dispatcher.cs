using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;

namespace ImageFanReloaded.Controls;

public class Dispatcher : IDispatcher
{
    public Dispatcher(Avalonia.Threading.Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Invoke(Action callback)
    {
        try
        {
            _dispatcher.Invoke(callback);
        }
        catch (TaskCanceledException)
        {
        }
    }

    #region Private

    private readonly Avalonia.Threading.Dispatcher _dispatcher;

    #endregion
}
