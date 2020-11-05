using System;
using System.Drawing;
using System.Linq;

namespace GL2DecodingLibrary
{
    public class CSX
    {
        public CSX(string filename, bool replaceFillColorAsTransparent)
        {
            this.replaceFillColorAsTransparent = replaceFillColorAsTransparent;
            var r = new Reader(filename);
            if (r.IsEnd())
                return;
            var colorCount = r.ReadInt();
            fillColor = r.ReadBGRA();
            colors = new Color[colorCount].Select(x => r.ReadBGRA()).ToArray();
            width = r.ReadInt();
            height = r.ReadInt();
            byteLineIndices = new int[height + 1].Select(x => r.ReadInt()).ToArray();
            var bytesCount = byteLineIndices.LastOrDefault();
            bytes = r.ReadByte(bytesCount);
        }

        public bool replaceFillColorAsTransparent;
        
        public Color[] colors;
        public Color fillColor;

        public int[] byteLineIndices;
        public byte[] bytes;

        public int width;
        public int height;

        public Color[,] DecodeColorGrid()
        {
            if (colors == null)
                return null;
            var fillColorIndex = -1;
            for (int i = 0; i < colors.Length; i++)
                if (colors[i] == fillColor)
                {
                    fillColorIndex = i;
                    break;
                }

            var pixelIndices = new byte[width * height].Select(x => fillColorIndex).ToArray();

            for (int y = 0; y < height; y++)
                DecodeLine(bytes, byteLineIndices[y], pixelIndices, y * width, width,
                    byteLineIndices[y + 1] - byteLineIndices[y]);

            var pixels = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var xyIndex = x + y*width;
                    var colorIndex = pixelIndices[xyIndex];

                    Color color;
                    if (colorIndex < 0 || colorIndex == fillColorIndex)
                        color = replaceFillColorAsTransparent ? Color.Transparent : fillColor;
                    else if (colorIndex < colors.Length)
                        color = colors[colorIndex];
                    else
                        color = Color.Black;

                    pixels[x, y] = color;
                }
            }
            return pixels;
        }

        public Color[] DecodeColorArray()
        {
            if (colors == null)
                return null;
            
            var fillColorIndex = -1;
            for (byte i = 0; i < colors.Length; i++)
                if (colors[i] == fillColor)
                {
                    fillColorIndex = i;
                    break;
                }

            var pixelIndices = new byte[width * height].Select(x => fillColorIndex).ToArray();

            for (int y = 0; y < height; y++)
                DecodeLine(bytes, byteLineIndices[y], pixelIndices, y * width, width, byteLineIndices[y + 1] - byteLineIndices[y]);

            var pixels = new Color[width*height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var xyIndex = x + y* width;
                    var colorIndex = pixelIndices[xyIndex];

                    Color color;
                    if (colorIndex < 0)
                        color = fillColor;
                    else if (colorIndex < colors.Length)
                        color = colors[colorIndex];
                    else
                        color = Color.Black;

                    pixels[x + y*width] = color;
                }
            }
            return pixels;
        }

        //Magic
        void DecodeLine(
            byte[] bytes, int byteIndex,
            int[] pixels, int pixelIndex,
            int widthLeft,
            int byteCount)
        {
            var startPixelIndex = pixelIndex;
            while (widthLeft > 0 && byteCount > 0)
            {
                int x = bytes[byteIndex];
                byteIndex++;
                byteCount--;
                switch (x)
                {
                    case 107: //wtf why need it?
                        pixels[pixelIndex] = bytes[byteIndex];
                        if (pixelIndex != startPixelIndex)
                            pixels[pixelIndex - 1] = bytes[byteIndex];
                        byteIndex++;
                        byteCount--;
                        pixelIndex++;
                        widthLeft--;
                        break;
                    case 105: //pen transparent
                        pixelIndex++;
                        widthLeft--;
                        break;
                    case 106: //fill color
                        x = Math.Min(widthLeft, bytes[byteIndex + 1]);
                        for (int i = 0; i < x; i++)
                            pixels[pixelIndex + i] = bytes[byteIndex];

                        if (pixelIndex != startPixelIndex)
                            pixels[pixelIndex - 1] = bytes[byteIndex];
                        byteCount -= 2;
                        byteIndex += 2;
                        pixelIndex += x;
                        widthLeft -= x;
                        break;
                    case 108: //fill transparent
                        x = Math.Min(bytes[byteIndex], widthLeft);
                        byteIndex++;
                        byteCount--;
                        pixelIndex += x;
                        widthLeft -= x;
                        break;
                    default: //pen color
                        pixels[pixelIndex] = x;
                        pixelIndex++;
                        widthLeft--;
                        break;
                }
            }
        }
    }
}
