using MyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConvertTool
{
	internal static class Program
	{
		#region FIELDS
		private static bool blnIncludeSubdirs = false;
		private static string filenames;
		private static string src;
		private static string dst;
		private static int totalCnt;
		private const string convertPath = "Converted";
		#endregion
		private static void Main(string[] args)
		{
			if (CheckArgs(args))
			{
				if (Path.GetExtension(filenames)?.Length == 0) filenames += "\\*.ar3";
				string path = Path.GetDirectoryName(filenames);
				var pattern = Path.GetFileName(filenames);
				var outputPath = path + "-" + convertPath;
				if (path.Length == 0) { path = "."; }
				var files = Directory.GetFiles(path, pattern, (blnIncludeSubdirs) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				if (files.Length == 0) Console.WriteLine($"No files found: {filenames}");
				totalCnt = 0;
				foreach (var file in files)
				{
					try
					{
						RouteConvertTool(path, outputPath, file.Substring(path.Length), src, dst);
					}
					catch (Exception ee) { Console.WriteLine($"Error: {ee.Message} {ee.InnerException?.Message}"); }
				}
				Console.WriteLine($"Total {totalCnt}/{files?.Length} file(s) converted.");
			}
		}

		//private static string SetOutput(string convertPath, string filename)
		//{
		//	var outfile = "";// filename.Replace(".", $" {convertPath}.");
		//	if (Path.IsPathRooted(filename))
		//	{
		//	}
		//	else
		//	{
		//		outfile = My.CheckPath(convertPath, filename);
		//	}
		//	return outfile;
		//}

		private static void RouteConvertTool(string inputPath, string outputPath, string filename, string src, string dst)
		{
			var cnt = 0;
			var line = "";
			var inputfilename = inputPath + filename;
			var outputfilename = My.CheckPath(outputPath,filename);
			using (TextReader reader = new StreamReader(inputfilename))
			using (TextWriter writer = new StreamWriter(outputfilename))
			{
				while ((line = reader.ReadLine()) != null)
				{
					var output = line.Replace(src, dst);
					writer.WriteLine(output);
					if (line != output) ++cnt;
				}
			}
			Console.ForegroundColor = (cnt == 0) ? ConsoleColor.Red : ConsoleColor.White;
			Console.WriteLine($"Converting to: {outputfilename}.");
			totalCnt += (cnt > 0) ? 1 : 0;
		}

		private static bool CheckArgs(string[] args)
		{
			foreach (var arg in args)
			{
				if (arg.Contains("?")) { }
				else if (arg.IndexOf("/s", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					blnIncludeSubdirs = true;
				}
			}

			if (args.Length >= 3)
			{
				filenames = args[0];
				src = args[1];
				dst = args[2];
				return true;
			}
			ShowSyntaxHelp();
			return false;
		}

		private static void ShowSyntaxHelp()
		{
			const string help = @"
			Syntax: RouteConvertTool <filenames> <src> <dst> [/?] [/s] [/m:<methodename>]
			
			Where: 
			filenames = Routefile(s) to convert.
			src dst   = Replace all occurance of src by dst.
			/? = Show help.
			/s = Include subdirectories in filename search.
			/m = Use methodename to convert file.

			When a parameter contain whitespaces, you can enclose them in quotes.

			Example: RouteConvert holten.ar3 [0]:370 [0]:0
			";
			Console.WriteLine(help.Replace('\t', '+'));
		}
	}
}
