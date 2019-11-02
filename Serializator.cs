using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RevitServerExport
{
	public static class Serializator
	{
		private static BinaryFormatter _bin = new BinaryFormatter();

		public static void Serialize(string pathOrFileName, object objToSerialise)
		{
			using (Stream stream = File.Open(pathOrFileName, FileMode.Create))
			{
				try
				{
					_bin.Serialize(stream, objToSerialise);
				}
				catch (SerializationException e)
				{
					Console.WriteLine("Failed to serialize. Reason: " + e.Message);
					throw;
				}
			}
		}

		public static T Deserialize<T>(string pathOrFileName)
		{
			T items;

			using (Stream stream = File.Open(pathOrFileName, FileMode.Open))
			{
				try
				{
					items = (T)_bin.Deserialize(stream);
				}
				catch (SerializationException e)
				{
					Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
					throw;
				}
			}

			return items;
		}
		#region old XML serialization
					   //if (!File.Exists(file))
					   //{
						  // //Insert TreeBuilder logic here
						  // ParallelLoopResult res = Parallel.ForEach(Nodes, i =>
						  // {
							 //  i.Children = NodeViewer.FillingTree(root + folder, i.Text + "|");
							 //  foreach (var item in i.Children)
								//   item.Parent = i;

						  // });
						  // using (Stream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None))
						  // {
							 //  dcs.WriteObject(fs, Nodes);
						  // }
					   //}
					   //else
					   //{
						  // using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
						  // {
							 //  Nodes = dcs.ReadObject(fs) as ObservableCollection<Node>;
						  // }
					   //}
					   #endregion
	}

}
