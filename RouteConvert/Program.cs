// <copyright file="Program.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>
namespace RouteConvertTool
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using MyLib;

	/// <summary>
	/// This class contains the main entry point.
	/// </summary>
	internal static class Program
	{
		#region FIELDS
		/// <summary>
		/// Folder where converted files are stored.
		/// </summary>
		private const string ConvertPath = "_converted";

		/// <summary>
		/// IncludeSubdirs: when true subdirectories are included in file search. 
		/// </summary>
		private static bool blnIncludeSubdirs = false;

		/// <summary>
		/// File pattern to search files: *.ar3
		/// entered as first command line argument.  
		/// </summary>
		private static string filenames;

		/// <summary>
		/// Source string in input file to replace.
		/// </summary>
		private static string src;

		/// <summary>
		/// Destination string to replace source string in input file.
		/// </summary>
		private static string dst;

		/// <summary>
		/// Total number of files converted.
		/// </summary>
		private static int totalCnt;

		#endregion
		/// <summary>
		/// Main entry point.
		/// </summary>
		/// <param name="args">Command line arguments</param>
		private static void Main(string[] args)
		{
			if (CheckArgs(args))
			{
				if (Path.GetExtension(filenames)?.Length == 0) { filenames += "\\*.ar3"; }
				var inputPath = Path.GetDirectoryName(filenames);
				var pattern = Path.GetFileName(filenames);
				var outputPath = inputPath + ConvertPath;
				if (inputPath.Length == 0) { inputPath = "."; }
				var files = Directory.GetFiles(inputPath, pattern, blnIncludeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				if (files.Length == 0) { Console.WriteLine($"No files found: {filenames}"); }
				totalCnt = 0;
				foreach (var file in files)
				{
					try
					{
						RouteConvertTool(inputPath, outputPath, file.Substring(inputPath.Length), src, dst);
					}
					catch (Exception ee) { Console.WriteLine($"Error: {ee.Message} {ee.InnerException?.Message}"); }
				}

				Console.WriteLine($"Total {totalCnt}/{files?.Length} file(s) converted.");
			}
		}

		/// <summary>
		/// Convert input file to output file.
		/// </summary>
		/// <param name="inputPath">Path of input file</param>
		/// <param name="outputPath">Path of output file</param>
		/// <param name="filename">file name</param>
		/// <param name="src">source string</param>
		/// <param name="dst">destination string</param>
		private static void RouteConvertTool(string inputPath, string outputPath, string filename, string src, string dst)
		{
			var cnt = 0;
			var line = string.Empty;
			var inputfilename = inputPath + filename;
			var outputfilename = My.CheckPath(outputPath, filename);
			using (TextReader reader = new StreamReader(inputfilename))
			{
				using (TextWriter writer = new StreamWriter(outputfilename))
				{
					while ((line = reader.ReadLine()) != null)
					{
						var output = line.Replace(src, dst);
						writer.WriteLine(output);
						if (line != output) { ++cnt; }
					}
				}
			}

			Console.ForegroundColor = (cnt == 0) ? ConsoleColor.Red : ConsoleColor.White;
			Console.WriteLine($"Converting to: {outputfilename}.");
			totalCnt += (cnt > 0) ? 1 : 0;
		}

		/// <summary>
		/// Check command line arguments.
		/// </summary>
		/// <param name="args">argument lines</param>
		/// <returns>True when arguments are correct</returns>
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

		/// <summary>
		/// this syntax help is shown when /? is entered on command line.
		/// </summary>
		private static void ShowSyntaxHelp()
		{
			const string Help = @"
			Syntax: 

			RouteConvertTool <filenames> <src> <dst> [/?] [/s] [/m:<methodename>]
			
			Where: 
			filenames = Routefile(s) to convert.
			src dst   = Replace all occurance of src by dst.
			/? = Show help.
			/s = Include subdirectories in filename search.
			/m = Use methodename to convert file.

			When a parameter contain whitespaces, you can enclose them in quotes.

			Example: RouteConvert holten.ar3 [0]:370 [0]:0
			";
			Console.WriteLine(Help.Replace('\t', ' '));
		}
	}
}
