using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CSX_Decoder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Конвертер файлов: CSX to PNG");
            Console.WriteLine("Для конвертации перенести на значок файла");
            Console.WriteLine("by Alex Kasaurov");
            Console.WriteLine("");

            if (args.Length == 0)
            {
                var files = Directory.GetFiles(".", "*.csx", SearchOption.AllDirectories);
                Console.WriteLine("Files: " + files.Length);
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    Console.Write((i+1) + "/" + files.Length + " ");
                    ConvertFile(file);
                }
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                    {
                        ConvertFile(arg);
                    }
                    else if (Directory.Exists(arg))
                    {
                        var files = Directory.GetFiles(arg, "*.csx", SearchOption.AllDirectories);
                        Console.WriteLine("Files: " + files.Length);

                        for (var i = 0; i < files.Length; i++)
                        {
                            var file = files[i];
                            Console.Write((i+1) + "/" + files.Length + " ");
                            ConvertFile(file);
                        }
                    }
                }
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        
        static void ConvertFile(string path)
        {
            var ex = Path.GetExtension(path);
            if (ex == null) return;
            ex = ex.ToLower();
            switch (ex)
            {
            case ".csx":
                Convert_CSX_TO_PNG(path);
                break;
            }
        }
        
        private static void Convert_CSX_TO_PNG(string path)
        {
            Console.Write(path + "..");
            var csx = new CSX(path, true);
            Console.Write(" [" + csx.width + "x" + csx.height + "] ..");
            var colors = csx.DecodeColorGrid();
            if (colors == null)
            {
                Console.WriteLine("Ignore");
            }
            else
            {
                var bitmap = new Bitmap(csx.width, csx.height, PixelFormat.Format32bppArgb);
                for (int x = 0; x < csx.width; x++)
                for (int y = 0; y < csx.height; y++)
                    bitmap.SetPixel(x, y, colors[x, y]);

                var endPath = ReplaceExtension(path, ".csx.png");
                bitmap.Save(endPath, ImageFormat.Png);
                Console.WriteLine("Ok");
            }
        }

        private static string ReplaceExtension(string path, string ex)
        {
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            return dir + "\\" + name + ex;
        }
    }
}