using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;
using System;

readonly struct DrawCommand
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

    static byte count;
    public static DrawCommand
      Rectangle(int x, int y, int w, int h, Color32 color)
      {
          if (w == 320 && h == 340) (w, h) = (0, 0);
          return new DrawCommand(0, x, y, w, h, color, color);
      }
}

public sealed class Tester : MonoBehaviour
{
    #region Public properties

    [field:SerializeField, HideInInspector] public Mesh QuadMesh { get; set; }
    [field:SerializeField, HideInInspector] public Material QuadMaterial { get; set; }

    #endregion

    #region Predefined values

    static readonly string[] DeviceTypeNames =
      { "Headless", "Beta M8", "Production M8" };

    static class CommandCode
    {
        public const int DrawChar = 0xfd;
        public const int DrawRect = 0xfe;
        public const int DeviceInfo = 0xff;
    }

    #endregion

    #region Serial communication

    SerialPort _port;
    SlipParser _slip = new SlipParser();

    string DetectPort()
    {
        foreach (var name in SerialPort.GetPortNames())
            if (name.StartsWith("/dev/tty.usbmodem")) return name;
        return null;
    }

    void OpenPort(string name)
    {
        _port = new SerialPort();
        _port.PortName = name;
        _port.BaudRate = 115200;
        _port.DataBits = 8;
        _port.Parity = Parity.None;
        _port.StopBits = StopBits.One;
        _port.Handshake = Handshake.None;
        _port.ReadTimeout = 5;
        _port.WriteTimeout = 5;

        _port.Open();
        _port.Write(new [] {'E', 'R'}, 0, 2);

        Debug.Log($"Serial port: {name}");
    }

    void PrintDeviceInfo(ReadOnlySpan<byte> data)
    {
        Debug.Log($"Device type: {DeviceTypeNames[data[1]]}");
        Debug.Log($"Firmware version: {data[2]}.{data[3]}.{data[4]}");
        Debug.Log(data[5] == 1 ? "Large mode" : "Small mode");
    }

    void OnMessageReceived(ReadOnlySpan<byte> data)
    {
        switch (data[0])
        {
            case CommandCode.DrawChar: PushCharacterCommand(data); break;
            case CommandCode.DrawRect: PushRectangleCommand(data); break;
            case CommandCode.DeviceInfo: PrintDeviceInfo(data); break;
        };
    }

    #endregion

    #region Draw commands

    Queue<DrawCommand> _drawCommands = new Queue<DrawCommand>();
    MaterialPropertyBlock _prop;

    void PushCharacterCommand(ReadOnlySpan<byte> data)
      => _drawCommands.Enqueue(DrawCommand.Character
           (data[1],
            data[2] | (data[3] << 8), data[4] | (data[5] << 8),
            new Color32(data[9], data[10], data[11], 0xff),
            new Color32(data[6], data[7], data[8], 0xff)));

    void PushRectangleCommand(ReadOnlySpan<byte> data)
      => _drawCommands.Enqueue(DrawCommand.Rectangle
           (data[1] | (data[2] << 8), data[3] | (data[4] << 8),
            data[5] | (data[6] << 8), data[7] | (data[8] << 8),
            new Color32(data[9], data[10], data[11], 0xff)));

    void SendDrawCommand(in DrawCommand cmd)
    {
        _prop.SetInteger("_Code", cmd.code);
        _prop.SetVector("_Coords", new Vector4(cmd.x, cmd.y, cmd.w, cmd.h));
        _prop.SetColor("_Background", cmd.bg);
        _prop.SetColor("_Foreground", cmd.fg);
        var rparams = new RenderParams(QuadMaterial) { matProps = _prop };
        Graphics.RenderMesh(rparams, QuadMesh, 0, Matrix4x4.identity);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _prop = new MaterialPropertyBlock();
        _slip.OnReceived = OnMessageReceived;
        OpenPort(DetectPort());
    }

    void OnDestroy()
    {
        _port?.Close();
        _port = null;
    }

    void Update()
    {
        while (_port.BytesToRead > 0) _slip.FeedByte(_port.ReadByte());
        while (_drawCommands.Count > 0) SendDrawCommand(_drawCommands.Dequeue());
    }

    #endregion
}

