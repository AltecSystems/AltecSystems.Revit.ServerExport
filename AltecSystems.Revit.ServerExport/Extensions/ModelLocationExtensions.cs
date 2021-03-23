using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Model;
using System;
using System.IO;

namespace AltecSystems.Revit.ServerExport.Extensions
{
    public static class ModelLocationExtensions
    {
		public static string GetUserVisiblePathFromModelLocation(this ModelLocation modelLocation)
		{
			if (modelLocation.Type != ModelLocationType.Server)
			{
				throw new NotSupportedException("ERROR: GetUserVisiblePathFromModelLocation cannot handle non-server ModelLocations.");
			}
			return string.Format("{0}{1}{2}{3}", "RSN://", modelLocation.CentralServer, Path.AltDirectorySeparatorChar, modelLocation.RelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
		}
	}
}
