using System;

namespace ImageFanReloaded.Core.Controls;

public interface IDispatcher
{
    void Invoke(Action callback);
}
