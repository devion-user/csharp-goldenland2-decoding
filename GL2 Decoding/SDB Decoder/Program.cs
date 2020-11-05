using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SDB_Decoder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Конвертер файлов: SDB to TXT");
            Console.WriteLine("Для конвертации перенести на значок файла");
            Console.WriteLine("by Alex Kasaurov");
            Console.WriteLine("");

            if (args.Length == 0)
            {
                var files = Directory.GetFiles(".", "*.sdb", SearchOption.AllDirectories);
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
                        var files = Directory.GetFiles(arg, "*.sdb", SearchOption.AllDirectories);
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
            case ".sdb":
                Convert_SDB_TO_PNG(path);
                break;
            }
        }
        
        private static void Convert_SDB_TO_PNG(string path)
        {
            Console.Write(path + "..");
            var sdb = new SDB(path);
            var lines = sdb.items.Select(x => x.id + "---" + x.value).ToArray();

            var endPath = ReplaceExtension(path, ".sdb.txt");
            File.WriteAllLines(endPath, lines);
            Console.WriteLine("Ok");
        }

        private static string ReplaceExtension(string path, string ex)
        {
            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            return dir + "\\" + name + ex;
        }
    }
}