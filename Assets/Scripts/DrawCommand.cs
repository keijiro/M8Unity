using UnityEngine;

namespace M8 {

public readonly struct DrawCommand
{
    public readonly int code;
    public readonly int x, y, w, h;
    public readonly Color32 bg;
    public readonly Color32 fg;

    public DrawCommand
      (int code, int x, int y, int w, int h, Color32 bg, Color32 fg)
    {
        this.code = code;
        this.x = x; this.y = y;
        this.w = w; this.h = h;
        this.bg = bg; this.fg = fg;
    }

    public static DrawCommand
      Character(int code, int x, int y, Color32 bg, Color32 fg)
      => new DrawCommand(code, x, y, 8, 8, bg, fg);

    public static DrawCommand
      Rectangle(int x, int y, int w, int h, Color32 color)
    {
        if (w == 320 && h == 340) (w, h) = (0, 0);
        return new DrawCommand(0, x, y, w, h, color, color);
    }
}

} // namespace M8
