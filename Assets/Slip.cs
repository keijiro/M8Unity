using System;

public sealed class SlipParser
{
    byte[] _buffer = new byte[4096];
    int _count = 0;

    const byte ByteEnd = 0xc0;
    const byte ByteEsc = 0xdb;
    const byte ByteEscEnd = 0xdc;
    const byte ByteEscEsc = 0xdd;

    public delegate void MessageCallback(ReadOnlySpan<byte> data);

    public MessageCallback OnReceived;

    bool _escaped;

    void Append(byte b) => _buffer[_count++] = b;

    public void FeedBytes(ReadOnlySpan<byte> data)
    {
        foreach (var b in data) FeedByte(b);
    }

    public void FeedByte(int data)
      => FeedByte((byte)data);

    public void FeedByte(byte data)
    {
        if (_escaped)
        {
            if (data == ByteEscEnd)
            {
                Append(ByteEnd);
            }
            else if (data == ByteEscEsc)
            {
                Append(ByteEsc);
            }
            _escaped = false;
        }
        else
        {
            if (data == ByteEnd)
            {
                OnReceived(new Span<byte>(_buffer, 0, _count));
                _count = 0;
            }
            else if (data == ByteEsc)
            {
                _escaped = true;
            }
            else
            {
                Append(data);
            }
        }
    }
}
