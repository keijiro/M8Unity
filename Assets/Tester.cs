using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;
using System;

public sealed class Tester : MonoBehaviour
{
    [field:SerializeField] public Mesh QuadMesh { get; set; }
    [field:SerializeField] public Material TextMaterial { get; set; }

    SerialPort _port;
    SlipParser _slip = new SlipParser();
    MaterialPropertyBlock _prop;

    static readonly string[] DeviceTypeNames =
      { "Headless", "Beta M8", "Production M8" };

    List<(int c, int x, int y, Color32 fg, Color32 bg)> _textCommands =
      new List<(int c, int x, int y, Color32 fg, Color32 bg)>();

    void OnMessageReceived(ReadOnlySpan<byte> data)
    {
        if (data[0] == 0xff)
        {
            Debug.Log($"Device type: {DeviceTypeNames[data[1]]}");
            Debug.Log($"Firmware version: {data[2]}.{data[3]}.{data[4]}");
        }
        else if (data[0] == 0xfd)
        {
            var c = data[1];
            var x = data[2] | (data[3] << 8);
            var y = data[4] | (data[5] << 8);
            var fg = new Color32(data[6], data[7], data[8], 0xff);
            var bg = new Color32(data[9], data[10], data[11], 0xff);
            _textCommands.Add((c, x, y, fg, bg));
        }
    }

    void Start()
    {
        _slip.OnReceived = OnMessageReceived;

        _prop = new MaterialPropertyBlock();

        foreach (var name in SerialPort.GetPortNames())
            if (name.StartsWith("/dev/tty.usbmodem")) TestPort(name);
    }

    void OnDestroy()
    {
        _port?.Close();
        _port = null;
    }

    void Update()
    {
        while (_port.BytesToRead > 0) _slip.FeedByte(_port.ReadByte());
        foreach (var cmd in _textCommands) DrawCharacter(cmd);
    }

    void DrawCharacter(in (int c, int x, int y, Color32 fg, Color32 bg) cmd)
    {
        _prop.SetInteger("_Character", cmd.c);
        _prop.SetVector("_Position", new Vector2(cmd.x, cmd.y));
        _prop.SetColor("_Background", cmd.bg);
        _prop.SetColor("_Foreground", cmd.fg);
        var rparams = new RenderParams(TextMaterial);
        rparams.matProps = _prop;
        Graphics.RenderMesh(rparams, QuadMesh, 0, Matrix4x4.identity);
    }

    void TestPort(string name)
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

        Debug.Log($"{name} opened");

        _port.Write(new [] {'E', 'R'}, 0, 2);
    }
}

