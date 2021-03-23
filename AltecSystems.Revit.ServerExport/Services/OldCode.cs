//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AltecSystems.Revit.ServerExport.Services
//{
//	class Export2
//	{
//		public static void Export()
//		{
//			//"192.168.1.10" arg 1
//			//"E:\\ProgramData\\Revit server\\01_Bim-отдел\\Тест_Отделка.rvt" arg 2
//			var relativePath = "E:\\ProgramData\\Revit server\\01_Bim-отдел\\Тест_Отделка.rvt";
//			var hostId = "192.168.1.10";
//			//Autodesk.RevitServer.Enterprise.Common.ClientServer.ServiceContract.Model
//			var modelService = ProxyProvider.Instance.GetBufferedProxy<IModelService>(hostId);
//			ModelLocation modelLocation = new ModelLocation(hostId, relativePath, ModelLocationType.Server);

//			var strUserName = "RevitServerTool:DESKTOP-FP5GVOM:1";
//			var strSSO_UserName = string.Empty;

//			ServiceSessionToken sessionToken = new ServiceSessionToken(strUserName, strSSO_UserName, Environment.MachineName, Guid.NewGuid().ToString());
//			ModelIdentity modelIdentity = IdentifyModel(sessionToken, relativePath, true, hostId);

//			ServiceModelSessionToken modelSessionToken = CreateServiceModelSessionToken(modelIdentity, modelLocation, strUserName, strSSO_UserName, Environment.MachineName);

//			ServiceModelSessionToken serviceModelSessionToken = CreateServiceModelSessionToken(modelIdentity, modelLocation, strUserName, strSSO_UserName, Environment.MachineName);

//			var lockData = LockData(serviceModelSessionToken, hostId, out var episodeGuid);

//			modelService.Proxy.GetListOfModelDataFilesWithoutLocking(modelSessionToken, out var arrayList);

//			string arg = "RevitServerTool_ModelDataFileDownload_";
//			string path2 = string.Format("{0}{1}", arg, Guid.NewGuid().ToString("N"));
//			string targeFolder = Path.Combine(Path.GetTempPath(), path2);

//			foreach (var item in arrayList)
//			{
//				string sourceFile = GetSourceFileName(modelIdentity, item.ToString());

//				string targetFile = Path.Combine(targeFolder, Path.GetFileName(item.ToString()));
//				ServiceModelSessionToken token = CreateServiceModelSessionToken(modelIdentity, modelLocation, strUserName, strSSO_UserName, Environment.MachineName);

//				DownloadFile(token, sourceFile, targetFile, "", hostId, episodeGuid);

//				Console.WriteLine(item.ToString());
//			}
//			CreateRvt(targeFolder, modelIdentity, modelLocation);

//			var bufferProxy = GetBufferedProxy(hostId);

//			var testToken = CreateServiceModelSessionToken(modelIdentity, modelLocation, strUserName, strSSO_UserName, Environment.MachineName);

//			bufferProxy.Proxy.ListSubFoldersAndModels(sessionToken, "E:\\ProgramData\\Revit server\\01_Bim-отдел", out var list, out var list1);
//			//bufferProxy.Proxy.Folder
//			Console.ReadKey();
//		}

//		public static void CreateRvt(string targeFolder, ModelIdentity modelIdentity, ModelLocation modelLocation)
//		{
//			//CastleClassFactory.Instance.AddConfigFile("RevitServerToolCastle.config");
//			//var t = CastleClassFactory.Instance;
//			var sharedUtils = SharedUtils.Instance;
//			string strDestinationPath = "C:\\Users\\kurbatov\\Desktop\\rvtTemp\\test.rvt";
//			string centralModelFullPath = GetUserVisiblePathFromModelLocation(modelLocation);
//			var flag = sharedUtils.GenerateRvtFileFromModelDataFolder(targeFolder, DataFormatVersion.Latest, strDestinationPath, true, true, modelIdentity, centralModelFullPath);
//			//Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.Utils.ISharedUtils
//			//Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.Utils.ISharedUtils was found'
//		}

//		private static string GetUserVisiblePathFromModelLocation(ModelLocation modelLocation)
//		{
//			if (modelLocation.Type != ModelLocationType.Server)
//			{
//				throw new NotSupportedException("ERROR: GetUserVisiblePathFromModelLocation cannot handle non-server ModelLocations.");
//			}
//			return string.Format("{0}{1}{2}{3}", "RSN://", modelLocation.CentralServer, Path.AltDirectorySeparatorChar, modelLocation.RelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
//		}

//		public static string GetSourceFileName(ModelIdentity modelIdentity, string fileName)
//		{
//			return Path.Combine(modelIdentity.IdentityGUID.GUID.ToString(), fileName);
//		}


//		public static ModelIdentity IdentifyModel(ServiceSessionToken sessionToken, string modelPath, bool bUsedForAccess, string hostNode)
//		{
//			IClientProxy<IModelService> bufferedProxy = GetBufferedProxy(hostNode);
//			IModelService proxy = bufferedProxy.Proxy;
//			return proxy.IdentifyModel(sessionToken, modelPath, bUsedForAccess);
//		}

//		public static LockStatus LockData(ServiceModelSessionToken sessionToken, string hostNode, out EpisodeGuid creationDate)
//		{
//			uint lockOptions = 129u;
//			ModelVersion localModelVersion = new ModelVersion(new VersionNumber(0), new ModelHistoryCheckInfo(EpisodeGuid.Invalid));

//			IClientProxy<IModelService> bufferedProxy = GetBufferedProxy(hostNode);
//			IModelService proxy = bufferedProxy.Proxy;
//			return proxy.LockData(sessionToken, lockOptions, bAllowBecomeNonExclusive: true, localModelVersion, out creationDate);
//		}

//		private static IClientProxy<IModelService> GetBufferedProxy(string targetNode)
//		{
//			//if (!ServerInfo.SecureCommunication)
//			//{
//			// Провейдер возвращает инстанс с сервера
//			return ProxyProvider.Instance.GetBufferedProxy<IModelService>(targetNode);
//			//}
//			//return ProxyProvider.Instance.GetSecuredBufferedProxy<IModelService>(targetNode);
//		}

//		private static IClientProxy<IModelService> GetStreamedProxy(string targetNode)
//		{
//			//if (!ServerInfo.SecureCommunication)
//			//{
//			return ProxyProvider.Instance.GetStreamedProxy<IModelService>(targetNode);
//			//}
//			//return ProxyProvider.Instance.GetSecuredStreamedProxy<IModelService>(targetNode);
//		}

//		private static IClientProxy<IModelService> GetRoutedProxy(string viaNode, string targetNode)
//		{
//			//if (!ServerInfo.SecureCommunication)
//			//{
//			return ProxyProvider.Instance.GetRoutedProxy<ILocalService, IModelService>(viaNode, targetNode);
//			//}
//			//return ProxyProvider.Instance.GetSecuredRoutedProxy<ILocalService, IModelService>(viaNode, targetNode);
//		}

//		private static ServiceModelSessionToken CreateServiceModelSessionToken(ModelIdentity modelIdentity, ModelLocation modelLoc, string userName, string ssoUserName, string machineName)
//		{
//			ServiceModelSessionToken serviceModelSessionToken = new ServiceModelSessionToken(modelIdentity, userName, ssoUserName, machineName, Guid.NewGuid().ToString());
//			serviceModelSessionToken.ModelLocation = modelLoc;
//			return serviceModelSessionToken;
//		}

//		public static bool DownloadFile(ServiceSessionToken token, string sourceFile, string targetFile, string acceleratorNode, string hostNode, EpisodeGuid creationDate)
//		{
//			bool flag = false;
//			try
//			{

//				//IClientProxy<IModelService> clientProxy = ((ServerInfo.ConnectionState != ServerInfo.ConnectionStateEnum.Failover && !string.IsNullOrWhiteSpace(acceleratorNode) && !acceleratorNode.Equals(hostNode, StringComparison.CurrentCultureIgnoreCase)) ? GetRoutedProxy(acceleratorNode, hostNode) : GetStreamedProxy(hostNode));
//				IClientProxy<IModelService> clientProxy = GetStreamedProxy(hostNode);
//				//IClientProxy<IModelService> clientProxy = GetRoutedProxy(acceleratorNode, hostNode);
//				IModelService proxy = clientProxy.Proxy;

//				return PerformDownload(ref proxy, token, sourceFile, targetFile, creationDate);
//			}
//			catch (Exception ex)
//			{
//				throw;
//			}
//		}

//		private static bool PerformDownload(ref IModelService proxy, ServiceSessionToken sessionToken, string sourceFile, string targetFile, EpisodeGuid creationDate)
//		{
//			using (FileDownloadMessageStream fileDownloadMessageStream = proxy.DownloadFile(new FileDownloadRequestMessage(sessionToken, creationDate, sourceFile)))
//			{
//				if (fileDownloadMessageStream.Stream == null)
//				{

//					//throw new CommunicationException("Null stream returned");
//					throw new Exception();
//				}
//				string directoryName = Path.GetDirectoryName(targetFile);
//				if (!string.IsNullOrEmpty(directoryName))
//				{
//					Directory.CreateDirectory(directoryName);
//				}
//				DateTime now = DateTime.Now;
//				using (FileStream fileStream = File.Open(targetFile, FileMode.OpenOrCreate, FileAccess.Write))
//				{
//					byte[] array = new byte[16384];
//					int num = 0;
//					do
//					{
//						num = fileDownloadMessageStream.Stream.Read(array, 0, array.Length);
//						if (num > 0)
//						{
//							fileStream.Write(array, 0, num);
//						}
//					}
//					while (num > 0);
//				}
//				fileDownloadMessageStream.Stream.Close();
//			}
//			return true;
//		}
//	}
//}
