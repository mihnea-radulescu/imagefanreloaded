using System;

namespace ImageFanReloaded.Core.Keyboard;

public interface IKeyboardKey
{
	TKeyboardKey GetInstance<TKeyboardKey>() where TKeyboardKey : Enum;
}
