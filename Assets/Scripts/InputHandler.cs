using UnityEngine;

namespace M8 {

public static class Buttons
{
    public const int Edit   = 1;
    public const int Option = 1 << 1;
    public const int Right  = 1 << 2;
    public const int Start  = 1 << 3;
    public const int Select = 1 << 4;
    public const int Down   = 1 << 5;
    public const int Up     = 1 << 6;
    public const int Left   = 1 << 7;
}

public sealed class InputHandler
{
    static byte GetState()
    {
        var state = (byte)0;
        if (Input.GetKey(KeyCode.P)) state += Buttons.Edit;
        if (Input.GetKey(KeyCode.O)) state += Buttons.Option;
        if (Input.GetKey(KeyCode.L)) state += Buttons.Right;
        if (Input.GetKey(KeyCode.Period)) state += Buttons.Start;
        if (Input.GetKey(KeyCode.Comma)) state += Buttons.Select;
        if (Input.GetKey(KeyCode.K)) state += Buttons.Down;
        if (Input.GetKey(KeyCode.I)) state += Buttons.Up;
        if (Input.GetKey(KeyCode.J)) state += Buttons.Left;
        return state;
    }

    byte _prev;

    public byte CurrentState => _prev;

    public bool Update()
    {
        var state = GetState();
        var changed = (_prev != state);
        _prev = state;
        return changed;
    }
}

} // namespace M8
