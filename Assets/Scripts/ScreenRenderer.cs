using UnityEngine;
using System.Collections.Generic;

namespace M8 {

public sealed class ScreenRenderer
{
    Queue<DrawCommand> _buffer = new Queue<DrawCommand>();

    (Mesh mesh, Material material) _quad;

    public ScreenRenderer(Mesh quadMesh, Material quadMaterial)
      => _quad = (quadMesh, new Material(quadMaterial));

    public void Push(in DrawCommand cmd)
      => _buffer.Enqueue(cmd);

    public void DrawBuffered()
    {
        while (_buffer.Count > 0)
            Draw(_buffer.Dequeue());
    }

    public void Draw(in DrawCommand cmd)
    {
        var coords = new Vector4(cmd.x, cmd.y, cmd.w, cmd.h);
        _quad.material.SetInteger("_Code", cmd.code);
        _quad.material.SetVector("_Coords", coords);
        _quad.material.SetColor("_Background", cmd.bg);
        _quad.material.SetColor("_Foreground", cmd.fg);
        _quad.material.SetPass(0);
        Graphics.DrawMeshNow(_quad.mesh, Matrix4x4.identity);
    }
}

} // namespace M8
