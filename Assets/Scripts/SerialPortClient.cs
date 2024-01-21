using UnityEngine;
using System.IO.Ports;
using System;

namespace M8 {

public sealed class SerialPortClient : MonoBehaviour
{
    #region Public properties

    [field:SerializeField, HideInInspector] public Mesh _quadMesh;
    [field:SerializeField, HideInInspector] public Material _quadMaterial;

    #endregion

    #region Serial communication

    SerialPort _port;
    SlipParser _slip;
    CommandParser _parser;
    ScreenRenderer _renderer;
    InputHandler _input;

    void OpenPort(string name)
    {
        Debug.Log($"Serial port: {name}");
        SerialPortUtil.Configure(_port, name);
        _port.Open();
        _port.Write(new [] {'E', 'R'}, 0, 2);
    }

    void OnMessageReceived(ReadOnlySpan<byte> message)
    {
        if (_parser.IsDrawCommand(message))
        {
            _renderer.Push(_parser.MakeDrawCommand(message));
            return;
        }

        if (_parser.IsDeviceInfo(message))
        {
            _parser.PrintDeviceInfo(message);
            return;
        }

        // Unsupported message
    }

    void SendInput()
    {
        _port.Write(new [] {(byte)'C', _input.CurrentState}, 0, 2);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _port = new SerialPort();
        _slip = new SlipParser();
        _parser = new CommandParser();
        _renderer = new ScreenRenderer(_quadMesh, _quadMaterial);
        _input = new InputHandler();

        _slip.OnReceived = OnMessageReceived;
        OpenPort(SerialPortUtil.DetectPort());
    }

    void OnDestroy()
      => _port?.Close();

    void Update()
    {
        while (_port.BytesToRead > 0) _slip.FeedByte(_port.ReadByte());
        _renderer.DrawBuffered();
        if (_input.Update()) SendInput();
    }

    #endregion
}

} // namespace M8
