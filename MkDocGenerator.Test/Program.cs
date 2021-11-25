using System;
using System.IO;
using System.Linq;
using MdDocGenerator;
using MdDocGenerator.Util;

namespace MkDocGenerator.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var outputPath = ConfigHelper.OutputPath;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            outputPath = new DirectoryInfo(outputPath).FullName;
            Directory.Delete(outputPath, true);
            var files = Directory.GetFiles(ConfigHelper.SourcePath, ConfigHelper.Dll, SearchOption.AllDirectories)
                .ToList();
            Console.WriteLine("开始生成Markdown文档...");
            ApiDocProfile.Instance().Output(files, outputPath);
            Console.WriteLine("Markdown文档生成完成");
            Console.ReadKey();
        }
    }
}