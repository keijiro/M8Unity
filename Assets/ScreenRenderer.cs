using UnityEngine;
using System.Collections.Generic;

namespace M8 {

public sealed class ScreenRenderer
{
    Queue<DrawCommand> _buffer = new Queue<DrawCommand>();
    MaterialPropertyBlock _props = new MaterialPropertyBlock();

    (Mesh mesh, Material material) _quad;

    public ScreenRenderer(Mesh quadMesh, Material quadMaterial)
      => _quad = (quadMesh, quadMaterial);

    public void Push(in DrawCommand cmd)
      => _buffer.Enqueue(cmd);

    public void DrawBuffered()
    {
        while (_buffer.Count > 0)
            Draw(_buffer.Dequeue());
    }

    public void Draw(in DrawCommand cmd)
    {
        _props.SetInteger("_Code", cmd.code);
        _props.SetVector("_Coords", new Vector4(cmd.x, cmd.y, cmd.w, cmd.h));
        _props.SetColor("_Background", cmd.bg);
        _props.SetColor("_Foreground", cmd.fg);
        var rparams = new RenderParams(_quad.material) { matProps = _props };
        Graphics.RenderMesh(rparams, _quad.mesh, 0, Matrix4x4.identity);
    }
}

} // namespace M8
