/*using System;
using System.ComponentModel;
using System.Text;

namespace GL2DecodingLibrary
{
    public class PAK
    {
        
        public PAK(string filename)
        {
            var win1251 = Encoding.GetEncoding("windows-1251");
            
            var file = new Reader(filename);
            var header = file.ReadChar1(4, win1251);
            if (header.ToUpper() != "PAK ")
                throw new Exception("not converted");

            file.ReadUint();
            var fileCount = file.ReadUint();
            
            items = new Item[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                items[i] = new Item
                {
                    path = file.ReadString(),
                    fileSize = file.ReadInt(),
                    fileOffset = file.ReadInt()
                };
            }

            // var c = file.position;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                file.position = item.fileOffset;
                
                item.bytes = file.ReadByte(item.fileSize);
                var bytes2 = new byte[item.bytes.Length];
                
                for (int j = 0; j < item.bytes.Length; j++)
                {
                    var b = item.bytes[j];
                    bytes2[(j - 4 + item.bytes.Length)%item.bytes.Length] = b;
                }

                item.bytes = bytes2;
            }

        }

        public Item[] items;
        public class Item
        {
            public string path;
            public int fileSize;
            public int fileOffset;
            public byte[] bytes;

            public override string ToString()
            {
                return "[" + fileOffset.ToString("N") + " - " + fileSize.ToString("N")  + " - " + (fileOffset + fileSize).ToString("N") +  "] " + path;
            }
        }
    }
}*/