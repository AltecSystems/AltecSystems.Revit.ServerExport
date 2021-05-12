using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Model;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.ModelStorage;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.OleFile;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.Utils;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.VersionManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltecSystems.Revit.ServerExport.Services
{
    class RvtService
    {
		public bool GenerateRvtFileFromModelDataFolder(string strModelDataPath, DataFormatVersion dataFormatVersion, string strRvtFilePath, bool bCreateLocal, bool bOverwrite, ModelIdentity centralIdentity, string centralModelFullPath)
		{
			if (File.Exists(strRvtFilePath))
			{
				if (!bOverwrite)
				{
					return false;
				}
				File.Delete(strRvtFilePath);
			}
			IDictionary<string, string> nonElemStreamFiles = new Dictionary<string, string>();
			IDictionary<int, string> elemStreamFiles = new Dictionary<int, string>();
			IDictionary<int, string> steelIncrementStreamFiles = new Dictionary<int, string>();
			using (IModelDataVersionManager modelDataVersionManager = CreateModelDataVersionManager(strModelDataPath, dataFormatVersion))
			{
				if (!modelDataVersionManager.GetLatestStreamFiles(ref nonElemStreamFiles, ref elemStreamFiles, ref steelIncrementStreamFiles))
				{
					return false;
				}
			}
			if (!RvtFile.GenerateRvtFileFromModelFolder(nonElemStreamFiles, elemStreamFiles, steelIncrementStreamFiles, dataFormatVersion, strRvtFilePath))
			{
				return false;
			}
            if (bCreateLocal && (object)centralIdentity != null && centralIdentity.isValid())
            {
                try
                {
					var basicFileInfo = ReadFromRVTFile(strRvtFilePath);
					if (basicFileInfo != null)
                    {
						basicFileInfo.AllLocalChangesSavedToCentral = false;
						basicFileInfo.Author = "Autodesk Revit";
						basicFileInfo.CentralIdentity = ModelIdentity.NewModelIdentity;
						basicFileInfo.CentralPath = "";
						basicFileInfo.Format = "2019";
						basicFileInfo.Identity = ModelIdentity.NewModelIdentity;
						basicFileInfo.IsLT = false;
						basicFileInfo.IsSingleUserCloudModel = false;
						basicFileInfo.IsWorkshared = false;
						basicFileInfo.LocaleWhenSaved = "RUS";
						basicFileInfo.OpenWorksetDefault = 1;
						basicFileInfo.SavedPath = "";
						basicFileInfo.Username = "";
						basicFileInfo.LatestCentralVersion = 1;
						basicFileInfo.UniqueDocumentVersionSequence = 1;
						basicFileInfo.UniqueDocumentVersionGUID = GUIDValue.NewGUIDValue();
						basicFileInfo.LatestCentralEpisodeGUID = GUIDValue.NewGUIDValue();
						basicFileInfo.WorksharingState = WorksharingState.WS_Central;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return true;
		}

		public bool WriteToRVTFile(string rvtFilePath, BasicFileInfo basicFileInfo)
		{
			if (!File.Exists(rvtFilePath))
			{
				return false;
			}
			StorageMode grfMode = StorageMode.ReadWrite | StorageMode.ShareExclusive;
			OleRootStorage oleRootStorage = new OleRootStorage();
			string text = null;
			try
			{
				if (!oleRootStorage.Open(rvtFilePath, (int)grfMode))
				{
					return false;
				}
				var modelDataStreamIdentifier = new ModelDataStreamIdentifier(ModelDataStreamType.mdstBasicFileInfo);
				OleStream oleStream = oleRootStorage.OpenStream(modelDataStreamIdentifier.StreamName, grfMode);
				text = Path.GetTempFileName();
				if (!WriteToStreamFile(text, basicFileInfo))
				{
					return false;
				}
				if (!oleStream.Import(text))
				{
					return false;
				}
				oleStream.Commit();
				oleRootStorage.Commit();
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				if (text != null)
				{
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
				oleRootStorage.Close(isSave: true);
			}
			return true;
		}

		public BasicFileInfo ReadFromRVTFile(string rvtFilePath)
		{
			if (!File.Exists(rvtFilePath))
			{
				return null;
			}
			StorageMode grfMode = StorageMode.ReadWrite | StorageMode.ShareExclusive;
			OleRootStorage oleRootStorage = new OleRootStorage();
			BasicFileInfo basicFileInfo = null;
			string text = null;
			try
			{
				if (!oleRootStorage.Open(rvtFilePath, (int)grfMode))
				{
					return null;
				}
				var modelDataStreamIdentifier = new ModelDataStreamIdentifier(ModelDataStreamType.mdstProjectInformation);
				OleStream oleStream = oleRootStorage.OpenStream(modelDataStreamIdentifier.StreamName, grfMode);
				text = Path.GetTempFileName();
				if (!oleStream.Export(text))
				{
					return null;
				}
				return ReadFromStreamFile(text);
			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				if (text != null)
				{
					try
					{
						File.Delete(text);
					}
					catch (Exception)
					{
					}
				}
				oleRootStorage.Close(isSave: false);
			}
		}

		protected BasicFileInfo ReadFromStreamFile(string basicFileInfoFilename)
		{
			if (!File.Exists(basicFileInfoFilename))
			{
				return null;
			}
			BasicFileInfo basicFileInfo = new BasicFileInfo();
			
			using (BinaryReader binaryReader = new BinaryReader(File.Open(basicFileInfoFilename, FileMode.Open), Encoding.Unicode)) 
			{
				try
				{
					basicFileInfo.Version = (BasicFileInfoStreamVersion)binaryReader.ReadInt32();
					basicFileInfo.IsWorkshared = binaryReader.ReadBoolean();
					basicFileInfo.WorksharingState = (WorksharingState)binaryReader.ReadByte();
					basicFileInfo.Username = readString(binaryReader);
					basicFileInfo.CentralPath = readString(binaryReader);
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_Format)
					{
						basicFileInfo.Format = readString(binaryReader);
						basicFileInfo.BuildVersion = readString(binaryReader);
					}
					else
					{
						basicFileInfo.BuildVersion = readString(binaryReader);
						basicFileInfo.Format = extractYearFromBuildVersion(basicFileInfo.BuildVersion);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LastSavePath)
					{
						basicFileInfo.SavedPath = readString(binaryReader);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_OpenWorksetDefault)
					{
						basicFileInfo.OpenWorksetDefault = binaryReader.ReadInt32();
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_IsLT)
					{
						basicFileInfo.IsLT = binaryReader.ReadBoolean();
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_CentralModelIdentity)
					{
						basicFileInfo.CentralIdentity = new ModelIdentity(new GUIDValue(new Guid(readString(binaryReader))));
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LocaleWhenSaved)
					{
						basicFileInfo.LocaleWhenSaved = readString(binaryReader);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_AllLocalChangesSavedToCentral)
					{
						basicFileInfo.AllLocalChangesSavedToCentral = binaryReader.ReadBoolean();
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LatestCentralVersionAndEpisode)
					{
						basicFileInfo.LatestCentralVersion = binaryReader.ReadInt32();
						basicFileInfo.LatestCentralEpisodeGUID = new GUIDValue(new Guid(readString(binaryReader)));
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_UniqueDocumentVersionIdentifier)
					{
						basicFileInfo.UniqueDocumentVersionGUID = new GUIDValue(Guid.Parse(readString(binaryReader)));
						basicFileInfo.UniqueDocumentVersionSequence = int.Parse(readString(binaryReader));
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_ModelIdentity)
					{
						basicFileInfo.Identity = new ModelIdentity(new GUIDValue(new Guid(readString(binaryReader))));
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_IsSingleUserCloudModel)
					{
						basicFileInfo.IsSingleUserCloudModel = binaryReader.ReadBoolean();
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_Author)
					{
						basicFileInfo.Author = readString(binaryReader);
						return basicFileInfo;
					}
					return basicFileInfo;
				}
				catch (Exception ex)
				{
					return null;
				}
				finally
				{
					binaryReader.Close();
				}
			} 
			
		}

		private static string readString(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num > 0)
			{
				byte[] bytes = reader.ReadBytes(2 * num);
				char[] chars = Encoding.Unicode.GetChars(bytes);
				return new string(chars);
			}
			return string.Empty;
		}

		private static string extractYearFromBuildVersion(string buildVersion)
		{
			int num = buildVersion.IndexOf('(');
			if (num >= 1)
			{
				string text = buildVersion.Substring(0, num - 1);
				int num2 = text.IndexOf("20");
				if (num2 >= 0)
				{
					return text.Substring(num2, 4);
				}
			}
			return "2009";
		}

		protected bool WriteToStreamFile(string basicFileInfoFilename, BasicFileInfo basicFileInfo)
		{
			bool flag = false;
			if (!File.Exists(basicFileInfoFilename))
			{
				return false;
			}
			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(basicFileInfoFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), Encoding.Unicode))
            {
				try
				{
					binaryWriter.Write((int)basicFileInfo.Version);
					binaryWriter.Write(basicFileInfo.IsWorkshared);
					binaryWriter.Write((byte)basicFileInfo.WorksharingState);
					writeString(binaryWriter, basicFileInfo.Username);
					writeString(binaryWriter, basicFileInfo.CentralPath);
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_Format)
					{
						writeString(binaryWriter, basicFileInfo.Format);
						writeString(binaryWriter, basicFileInfo.BuildVersion);
					}
					else
					{
						writeString(binaryWriter, basicFileInfo.BuildVersion);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LastSavePath)
					{
						writeString(binaryWriter, basicFileInfo.SavedPath);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_OpenWorksetDefault)
					{
						binaryWriter.Write(basicFileInfo.OpenWorksetDefault);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_IsLT)
					{
						binaryWriter.Write(basicFileInfo.IsLT);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_CentralModelIdentity)
					{
						writeString(binaryWriter, basicFileInfo.CentralIdentity.IdentityGUID.GUID.ToString());
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LocaleWhenSaved)
					{
						writeString(binaryWriter, basicFileInfo.LocaleWhenSaved);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_AllLocalChangesSavedToCentral)
					{
						binaryWriter.Write(basicFileInfo.AllLocalChangesSavedToCentral);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_LatestCentralVersionAndEpisode)
					{
						binaryWriter.Write(basicFileInfo.LatestCentralVersion);
						writeString(binaryWriter, basicFileInfo.LatestCentralEpisodeGUID.GUID.ToString());
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_UniqueDocumentVersionIdentifier)
					{
						writeString(binaryWriter, basicFileInfo.UniqueDocumentVersionGUID.GUID.ToString());
						writeString(binaryWriter, basicFileInfo.UniqueDocumentVersionSequence.ToString());
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_ModelIdentity)
					{
						writeString(binaryWriter, basicFileInfo.Identity.IdentityGUID.GUID.ToString());
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_IsSingleUserCloudModel)
					{
						binaryWriter.Write(basicFileInfo.IsSingleUserCloudModel);
					}
					if (basicFileInfo.Version >= BasicFileInfoStreamVersion.BFISV_Author)
					{
						writeString(binaryWriter, basicFileInfo.Author);
					}
					binaryWriter.Write(Environment.NewLine);
					binaryWriter.Write(basicFileInfo.ToString().ToCharArray());
					binaryWriter.Write(Environment.NewLine);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					binaryWriter.Close();
				}
			}
		}

		private static void writeString(BinaryWriter writer, string content)
		{
			writer.Write(content.Length);
			if (content.Length > 0)
			{
				writer.Write(content.ToCharArray());
			}
		}

		private IModelDataVersionManager CreateModelDataVersionManager(string modelDataPath, DataFormatVersion formatVersion)
		{
			IModelDataVersionManager modelDataVersionManager = new ModelDataVersionManager(modelDataPath, formatVersion);
			modelDataVersionManager.ModelPathUtils = SharedUtils.Instance.ModelPathUtils;
			return modelDataVersionManager;
		}
	}
}
