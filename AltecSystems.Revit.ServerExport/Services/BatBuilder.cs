using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using AltecSystems.Revit.ServerExport.Models;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class BatBuilder
    {
        public static void CheckLoaded((List<string>, List<string>) sourceAndDesination, string destination, string revitRoot)
        {
            var sources = sourceAndDesination.Item1;
            var destins = sourceAndDesination.Item2;
            var crackedTuple = sources.Zip(destins, (n, w) => new { Source = n, Dest = w });
            List<bool> loaded = new List<bool>();

            List<string> fails = new List<string>();
            if (!File.Exists(revitRoot + "RevitServerToolCommand\\RevitServerTool.exe"))
            {
                MessageBox.Show("Не удается найти Revit по указанному пути");
                return;
            }
            if (destins.Count == 0)
            {
                MessageBox.Show("Файлы не выбраны");
                return;
            }
            foreach (var file in crackedTuple)
            {
                if (File.Exists(destination + file.Dest))
                    loaded.Add(true);
                else
                {
                    loaded.Add(false);
                    fails.Add(file.Dest);
                }
            }
            if (loaded.Count(x => x == true) == destins.Count)
            {
                MessageBox.Show("Все файлы выгружены успешно");
            }
            else
            {
                var failureMessage = string.Join(Environment.NewLine, fails);
                MessageBox.Show("Не удалось загрузить:\n " + failureMessage);
            }
        }

        public static List<string> CombinePath(IEnumerable<Node> nodes, string root)
        {
            List<string> paths = new List<string>();
            List<string> folders = new List<string>();
            var buf = new List<string>();
            foreach (var node in nodes)
            {
                if (node.IsChecked == true || node.IsChecked == null)
                {
                    paths.Add(Path.Combine(root, node.Text));
                    paths.AddRange(CombinePath(node.Children, Path.Combine(root, node.Text)));
                }
            }

            return paths;
        }

        public static void CreateBat((List<string>, List<string>) sourceAndDesination, string ip, string destination)
        {
            //C:\Program Files\Autodesk\Revit 2019\RevitServerToolCommand
            //C:\Users\kolpakov\Desktop\ServerToolSource\RevitServerTool\bin\Debug\RevitServerTool.exe

            //string fileName = revitPath+"\\Export.bat";
            string fileName = Directory.GetCurrentDirectory() + "\\Export.bat";

            var sources = sourceAndDesination.Item1;
            var destins = sourceAndDesination.Item2;
            var crackedTuple = sources.Zip(destins, (n, w) => new { Source = n, Dest = w });

            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(fileName))
                {
                    AddText(fs, "chcp 1251\n");
                    AddText(fs, "RevitServerTool.exe");
                    AddText(fs, "\n\n");

                    foreach (var file in crackedTuple)
                    {
                        AddText(fs, "RevitServerTool\t");
                        AddText(fs, "L\t");

                        AddText(fs, "\"" + file.Source + "\"");
                        AddText(fs, "\t-s\t");
                        AddText(fs, ip);
                        AddText(fs, "\t-d\t");
                        AddText(fs, "\"" + destination + file.Dest + "\"");
                        AddText(fs, "\t-o");
                        AddText(fs, "\n");
                    }
                    //AddText(fs, "cmd /k");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //public static dynamic FindRevit(DirectoryInfo dir, string fileName)
        //{
        //	dynamic direc = dir;
        //	try
        //	{
        //		var files = dir.GetFiles(fileName).ToArray();
        //		foreach (var file in files)
        //		{
        //			try
        //			{
        //				//file.Delete();
        //				if (file.Name == fileName)
        //				{
        //					direc = file.Directory.FullName;
        //					return direc;
        //				}
        //			}
        //			catch
        //			{
        //				continue;
        //				//Console.WriteLine("Beda");
        //			}
        //		}
        //		foreach (var directory in dir.GetDirectories())
        //		{
        //			direc = FindRevit(directory, fileName);
        //		}
        //	}
        //	catch
        //	{
        //		//Console.WriteLine("Bolshaia beda");
        //		return null;
        //	}
        //	return direc;
        //}
        public static void RunBat(string revitPath)
        {
            string fileName = Directory.GetCurrentDirectory() + "\\Export.bat";
            //string fileName = revitPath +"\\Export.bat";
            string workingDirectory = revitPath + "\\RevitServerToolCommand";

            //string revitServerPath = @"E:\ProgramData\Revit|server\Онежская\Моделирование\АР\17-35_Секции_1-2_АР_ЦФХ.rvt";
            //string revitServerIp = "192.168.1.10";
            //string destinationDirectory = @"C:\data\17-35_Sekcii_1-2_AR_CFH.rvt";
            //string arguments = "L," + revitServerPath + ",-s," + revitServerIp + ",-d," + destinationDirectory + ",-o";
            //string test1 = "L," + @"E:\ProgramData\Revit|server\ГР00-002050_СкандинавияДом№8\Моделирование\АР\ГР00-002050_Паркинг_АР_ЦФХ.rvt" + ",-s," + "192.168.1.10" + ",-d," + @"C:\data\BiM\25_Выгрузка|файлов|с|Revit|Server\03_Выгруженные|файлы\GR00_002050_SkandinaviyaDom№8\Modelirovanie\AR\Shifr_Parking_AR_CFH.rvt" + ",-o";
            //string workingDirectory = @"C:\Program Files\Autodesk\Revit 2019\RevitServerToolCommand\";
            ProcessStartInfo processStart = new ProcessStartInfo()
            {
                WorkingDirectory = workingDirectory,
                FileName = fileName,
            };
            Process pr = new Process
            {
                StartInfo = processStart
            };
            pr.Start();
            pr.WaitForExit();
        }

        public static (List<string>, List<string>) Splitter(List<string> paths, string root)
        {
            List<string> files = new List<string>();
            List<string> roots = new List<string>();
            foreach (var path in paths)
            {
                if (path.EndsWith(".rvt"))
                {
                    files.Add(path);
                }
            }
            foreach (var das in files)
            {
                if (das.StartsWith(root))
                {
                    roots.Add(das.Substring(root.Length));
                }
            }
            return (files, roots);
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = Encoding.GetEncoding("windows-1251").GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        //private static string[] SearchRevit()
        //{
        //	try
        //	{
        //		string[] allFoundFiles = Directory.GetFiles("C:\\Program Files\\", "RevitServerTool.exe", SearchOption.AllDirectories);
        //		return allFoundFiles;
        //	}
        //	catch (Exception e)
        //	{
        //		MessageBox.Show(e.Message);
        //		throw;
        //	}
        //}
    }
}