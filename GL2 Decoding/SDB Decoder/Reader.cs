using System;
using System.Drawing;
using System.IO;
using System.Text;

internal class Reader
{
    public int position;
    public byte[] bytes;
    public Reader(string filename)
    {
        bytes = File.Exists(filename) ? File.ReadAllBytes(filename) : new byte[0];
        position = 0;
    }

    public Reader(byte[] bytes)
    {
        this.bytes = bytes;
        position = 0;
    }

    public bool IsEnd()
    {
        return position == bytes.Length;
    }

    #region Read data
    public byte ReadByte()
    {
        var b = bytes[position];
        position += 1;
        return b;
    }

    public byte[] ReadByte(int length)
    {
        var array = new byte[length];
        Buffer.BlockCopy(bytes, position, array, 0, length);
        position += length;
        return array;
    }

    public bool ReadBool()
    {
        var result = BitConverter.ToBoolean(bytes, position);
        position += sizeof(bool);
        return result;
    }

    public char ReadChar2()
    {
        var result = BitConverter.ToChar(bytes, position);
        position += sizeof(char);
        return result;
    }

    public int ReadInt()
    {
        var result = BitConverter.ToInt32(bytes, position);
        position += sizeof(int);
        return result;
    }

    public double ReadDouble()
    {
        var result = BitConverter.ToDouble(bytes, position);
        position += sizeof(double);
        return result;
    }

    public float ReadFloat()
    {
        var result = BitConverter.ToSingle(bytes, position);
        position += sizeof(float);
        return result;
    }

    public long ReadLong()
    {
        var result = BitConverter.ToInt64(bytes, position);
        position += sizeof(long);
        return result;
    }

    public short ReadShort()
    {
        var result = BitConverter.ToInt16(bytes, position);
        position += sizeof(short);
        return result;
    }

    public uint ReadUint()
    {
        var result = BitConverter.ToUInt32(bytes, position);
        position += sizeof(uint);
        return result;
    }

    public ulong ReadUlong()
    {
        var result = BitConverter.ToUInt64(bytes, position);
        position += sizeof(ulong);
        return result;
    }

    public ushort ReadUshort()
    {
        var result = BitConverter.ToUInt16(bytes, position);
        position += sizeof(ushort);
        return result;
    }

    public string ReadChar1(int byteLength, Encoding encoding)
    {
        var result = encoding.GetString(bytes, position, byteLength);
        position += byteLength;
        return result;
    }

    public string ReadString()
    {
        var length = ReadInt();
        var str = ReadChar1(length, EncodingVars.win1251);
        return str;
    }

    public Color ReadBGRA()
    {
        var b = ReadByte();
        var g = ReadByte();
        var r = ReadByte();
        var a = ReadByte();
        return Color.FromArgb(255-a, r, g, b);
    }
    #endregion
}