using UnityEngine;
using System;

namespace M8 {

public class CommandParser
{
    #region Predefined constants

    static readonly string[] DeviceTypeNames =
      { "Headless", "Beta M8", "Production M8" };

    static class CommandCode
    {
        public const int DrawChar = 0xfd;
        public const int DrawRect = 0xfe;
        public const int DeviceInfo = 0xff;
    }

    #endregion

    #region Public members

    public bool IsDrawCommand(ReadOnlySpan<byte> message)
      => message[0] == CommandCode.DrawChar ||
         message[0] == CommandCode.DrawRect;

    public bool IsDeviceInfo(ReadOnlySpan<byte> message)
      => message[0] == CommandCode.DeviceInfo;

    public DrawCommand MakeDrawCommand(ReadOnlySpan<byte> message)
      => message[0] == CommandCode.DrawChar ?
           MakeCharacterCommand(message) :
           MakeRectangleCommand(message);

    public void PrintDeviceInfo(ReadOnlySpan<byte> message)
    {
        Debug.Log($"Device type: {DeviceTypeNames[message[1]]}");
        Debug.Log($"Firmware version: {message[2]}.{message[3]}.{message[4]}");
        Debug.Log(message[5] == 1 ? "Large mode" : "Small mode");
    }

    #endregion

    #region Private members

    DrawCommand MakeCharacterCommand(ReadOnlySpan<byte> message)
      => DrawCommand.Character
           (message[1],
            message[2] | (message[3] << 8), message[4] | (message[5] << 8),
            new Color32(message[9], message[10], message[11], 0xff),
            new Color32(message[6], message[7], message[8], 0xff));

    DrawCommand MakeRectangleCommand(ReadOnlySpan<byte> message)
      => DrawCommand.Rectangle
           (message[1] | (message[2] << 8), message[3] | (message[4] << 8),
            message[5] | (message[6] << 8), message[7] | (message[8] << 8),
            new Color32(message[9], message[10], message[11], 0xff));

    #endregion
}

} // namespace M8
