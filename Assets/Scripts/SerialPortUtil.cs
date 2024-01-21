using System.IO.Ports;

namespace M8 {

static class SerialPortUtil
{
    public static string DetectPort()
    {
        foreach (var name in SerialPort.GetPortNames())
            if (name.StartsWith("/dev/tty.usbmodem")) return name;
        return null;
    }

    public static void Configure(SerialPort port, string name)
    {
        port.PortName = name;
        port.BaudRate = 115200;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.Handshake = Handshake.None;
        port.ReadTimeout = 5;
        port.WriteTimeout = 5;
    }
}

} // namespace M8
