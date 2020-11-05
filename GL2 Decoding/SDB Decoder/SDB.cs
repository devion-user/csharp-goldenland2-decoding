using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SDB
{
    public SDB(string path)
    {
        var file = new Reader(path);
        var header = file.ReadChar1(4, EncodingVars.win1251);
        var xor = header.ToUpper() != "SDB ";
        if (xor)
            file.position = 0;

        items = new List<Item>();
        while (!file.IsEnd())
        {
            var id = file.ReadInt();
            var textLength = file.ReadInt();
            var textBytes = file.ReadByte(textLength);
            if (xor)
                textBytes = textBytes.Select(x => (byte)(x ^ 0xAA)).ToArray();
            var text = EncodingVars.win1251.GetString(textBytes);
            items.Add(new Item(id, text));
        }
        this.path = path;
    }

    public string path;
    public List<Item> items = new List<Item>();

    [Serializable]
    public struct Item
    {
        public int id;
        public string value;

        public Item(int id, string value)
        {
            this.id = id;
            this.value = value;
        }

        public override string ToString()
        {
            return id + ":" + value;
        }
    }
}