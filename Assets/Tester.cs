using UnityEngine;
using System.IO.Ports;

public sealed class Tester : MonoBehaviour
{
    SerialPort _port;

    void Start()
    {
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
        while (_port.BytesToRead > 0)
            Debug.Log($"{_port.ReadByte():X2}");
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

        _port.Write(new [] {'E'}, 0, 1);
    }
}

