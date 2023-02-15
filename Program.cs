using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Filename_shuffler
{
	class Program
	{
		public static bool onlySame = true;
		static void Main()
		{
			try
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("Filename shuffler");
				Console.WriteLine("===================");
				Console.WriteLine(); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[0] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Shuffle 2 file's name "); Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(random)\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[1] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Shuffle 2 file's name "); Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(chosen)\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[2] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Shuffle 3 file's name "); Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(random)\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[3] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Shuffle custom number of file's name "); Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(random)\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[4] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Shuffle ALL file's name "); Console.ForegroundColor = ConsoleColor.Blue; Console.Write("(random)\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[5] "); Console.ForegroundColor = ConsoleColor.White; Console.Write("Only shuffle files with the same extension ");
				if (onlySame) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("(on)\n"); }
				else { Console.ForegroundColor = ConsoleColor.Red; Console.Write("(off)\n"); }
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[6] "); Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("Undo shuffle\n"); Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[Q] "); Console.ForegroundColor = ConsoleColor.Red; Console.Write("Exit program\n"); Console.ForegroundColor = ConsoleColor.White;
				ConsoleKeyInfo chosen = Console.ReadKey();
				if (chosen.Key == ConsoleKey.Q) { Environment.Exit(0); }
				else if (chosen.KeyChar == '6') { Undo(); }
				else if (chosen.KeyChar == '5')
				{
					switch (onlySame)
					{
						case false:
							onlySame = true;
							break;
						case true:
							onlySame = false;
							break;
					}
				}
				else if (chosen.KeyChar == '4') { ShuffleAll(); }
				else if (chosen.KeyChar == '3') { ShuffleCustom(); }
				else if (chosen.KeyChar == '2') { ShuffleCustom(3); }
				else if (chosen.KeyChar == '1') { ShuffleTwo(true); }
				else if (chosen.KeyChar == '0') { ShuffleTwo(false); }
				GC.Collect();
				Main();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Uh oh. An error happened. Message:");
				Console.WriteLine(ex.Message);
				Console.WriteLine("Press any key to go back to main menu...");
				Console.ReadKey();
                Main();
            }
		}
		static void Undo()
		{
			Console.Clear();
			Console.Write("Drag&drop undo file here: ");
			string unf = Console.ReadLine();
			unf = unf.Replace("\"", "");
			StreamReader undo = new StreamReader(unf);
			FileInfo text = new(unf);
			long lines = CountLinesLINQ(text);
			string[] files = new string[lines + 1];
			for (int i = 0; i < lines; i++)
			{
				files[lines - i] = undo.ReadLine();
			}
			for (long i = 0; i < lines; i++)
			{
				string file = files[i + 1];
				string fileO = file.Remove(file.IndexOf(">"));
				string fileS = file.Remove(0, file.IndexOf(">") + 1);
				Console.WriteLine("{0}=>{1}", fileO, fileS);
				File.Move(fileO, Directory.GetParent(fileO) + "TEMP");
				File.Move(fileS, fileO);
				File.Move(Directory.GetParent(fileO) + "TEMP", fileS);
			}
			undo.Close();
		}
		static void ShuffleAll()
		{
			int timesExecuted = 0;
			Console.Clear();
			Console.Write("Drag&drop target folder here: ");
			string fld = Console.ReadLine() + "\\";
			fld = fld.Replace("\"", "");
		undoAsk:
			Console.Write("Do you want to create an undo file? [Y/N]: ");
			bool creUnd = false;
			char creUndA = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
			Console.Write("\n");
			if (creUndA == 'Y') { creUnd = true; }
			else if (creUndA == 'N') { }
			else { goto undoAsk; }
		ReS:
			StreamWriter undo;
			if (timesExecuted > 0) { undo = CreateUndo(creUnd, fld + timesExecuted); }
			else { undo = CreateUndo(creUnd, fld); }
			Console.Write("Preparing to shuffle filenames... ");
			string[] files = Directory.GetFiles(fld);
			int fileCount = 0;
			foreach (var f in files) { fileCount++; }
			int i = fileCount;
			Random rnd = new Random();
			string file1;
			string file2;
			Console.ForegroundColor = ConsoleColor.Green; Console.Write("OK\n"); Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Shuffling filenames...");
			Console.ForegroundColor = ConsoleColor.Blue;
			while (i > 0)
			{
				file1 = files[rnd.Next(0, fileCount)];
				int pass = 10;
			getFile2:
                if (pass <= 0) { PassOut(Path.GetExtension(file1)); goto end; }
                file2 = files[rnd.Next(0, fileCount)];
				if (file1 == file2) { pass--; goto getFile2; }
                if (onlySame && Path.GetExtension(file1) != Path.GetExtension(file2)) { pass--; goto getFile2; }
                File.Move(file1, fld + "TEMP");
				File.Move(file2, file1);
				File.Move(fld + "TEMP", file2);
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("{0} <==> {1}", file1.Replace(fld, ""), file2.Replace(fld, ""));
				if (creUnd)
				{
					undo.WriteLine(file1 + ">" + file2);
				}
            end:;
                i--;
			}
			if (creUnd)
			{
				undo.Close();
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done!");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Shuffle again? [Y/N]: ");
			char again = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
			if (again == 'Y') { goto ReS; }
		}
		static void ShuffleCustom(int i = 2)
        {
            int timesExecuted = 0;
            Console.Clear();
            Console.Write("Drag&drop target folder here: ");
            string fld = Console.ReadLine() + "\\";
            fld = fld.Replace("\"", "");
			if (i != 3)
			{
			numGet:
				Console.WriteLine("If less than 2 files are given, 2 will be chosen.");
				Console.Write("How many files should be shuffled?: ");
				try { i = Convert.ToInt32(Console.ReadLine()); }
				catch { goto numGet; }
				if (i < 2) { i = 2; }
            }
        undoAsk:
            Console.Write("Do you want to create an undo file? [Y/N]: ");
            bool creUnd = false;
            char creUndA = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
            Console.Write("\n");
            if (creUndA == 'Y') { creUnd = true; }
            else if (creUndA == 'N') { }
            else { goto undoAsk; }
        ReS:
            StreamWriter undo;
            if (timesExecuted > 0) { undo = CreateUndo(creUnd, fld + timesExecuted); }
            else { undo = CreateUndo(creUnd, fld); }
            Console.Write("Preparing to shuffle filenames... ");
            string[] files = Directory.GetFiles(fld);
            int fileCount = 0;
            foreach (var f in files) { fileCount++; }
            Random rnd = new Random();
            string file1;
            string file2;
            Console.ForegroundColor = ConsoleColor.Green; Console.Write("OK\n"); Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Shuffling filenames...");
            Console.ForegroundColor = ConsoleColor.Blue;
            while (i > 0)
            {
                file1 = files[rnd.Next(0, fileCount)];
				int pass = 10;
            getFile2:
				if (pass <= 0) { PassOut(Path.GetExtension(file1)); goto end; }
                file2 = files[rnd.Next(0, fileCount)];
                if (file1 == file2) { pass--; goto getFile2; }
                if (onlySame && Path.GetExtension(file1) != Path.GetExtension(file2)) { pass--; goto getFile2; }
                File.Move(file1, fld + "TEMP");
                File.Move(file2, file1);
                File.Move(fld + "TEMP", file2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0} <==> {1}", file1.Replace(fld, ""), file2.Replace(fld, ""));
                if (creUnd)
                {
                    undo.WriteLine(file1 + ">" + file2);
                }
            end:;
                i--;
            }
            if (creUnd)
            {
                undo.Close();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Shuffle again? [Y/N]: ");
            char again = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
            if (again == 'Y') { goto ReS; }
        }
        static void ShuffleTwo(bool chosen)
		{
			Console.Clear();
			string file1 = "";
			string file2 = "";
			string fld = "";
			if (chosen)
			{
				Console.Write("Drag&drop file 1 here: ");
				file1 = Console.ReadLine();
				file1 = file1.Replace("\"", ""); Console.Write("\n");
				Console.Write("Drag&drop file 2 here: ");
				file2 = Console.ReadLine();
				file2 = file2.Replace("\"", ""); Console.Write("\n");
				fld = Path.GetDirectoryName(file1);
			}
			else
			{
				Console.Write("Drag&drop target folder here: ");
				fld = Console.ReadLine() + "\\";
				fld = fld.Replace("\"", ""); Console.Write("\n");
				Random rnd = new Random();
				string[] files = Directory.GetFiles(fld);
				int fileCount = 0;
				foreach (var f in files) { fileCount++; }
				file1 = files[rnd.Next(0, fileCount)];
				int pass = 10;
			getFile2:
                if (pass <= 0) { PassOut(Path.GetExtension(file1)); goto end; }
                file2 = files[rnd.Next(0, fileCount)];
				if (file1 == file2) { pass--; goto getFile2; }
				if (onlySame && Path.GetExtension(file1) != Path.GetExtension(file2)) { pass--; goto getFile2; }
			}
		undoAsk:
			Console.Write("Do you want to create an undo file? [Y/N]: ");
			bool creUnd = false;
			char creUndA = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
			Console.Write("\n");
			if (creUndA == 'Y') { creUnd = true; }
			else if (creUndA == 'N') { }
			else { goto undoAsk; }
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Shuffling filenames...");
			Console.ForegroundColor = ConsoleColor.Blue;
			StreamWriter undo;
			undo = CreateUndo(creUnd, fld);
			File.Move(file1, fld + "TEMP");
			File.Move(file2, file1);
			File.Move(fld + "TEMP", file2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0} <==> {1}", file1.Replace(fld, ""), file2.Replace(fld, ""));
			if (creUnd)
			{
				undo.WriteLine(file1 + ">" + file2);
                undo.Close();
            }
        end:;
            Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done! Press any key to go to main menu...");
			Console.ReadKey();
		}
		public static void PassOut(string ext)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Out of filename passes. Check if the folder contain another file with the {0} extension.", ext);
			Console.ReadKey();
		}
		public static StreamWriter CreateUndo(bool creUnd, string fld)
		{
			if (creUnd)
			{
			uf:
				Console.Write("Undo file name (will be saved to parent directory; don't add extension) [ENTER for default]: ");
				string uf = Console.ReadLine();
				if (uf.Contains("\\") || uf.Contains("/") || uf.Contains(":") || uf.Contains("*") || uf.Contains("?") || uf.Contains("\"") || uf.Contains("<") || uf.Contains(">") || uf.Contains("|")) { goto uf; }
				if (uf.Replace(" ", "") == "") { uf = "shuffle"; }
				StreamWriter undo = new StreamWriter(Directory.GetParent(fld) + uf + ".undo");
				return undo;
			}
			else
			{
				return null;
			}
		}
		static long CountLinesLINQ(FileInfo file)
	=> File.ReadLines(file.FullName).Count();
	}
}