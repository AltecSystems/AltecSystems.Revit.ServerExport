using AltecSystems.Revit.ServerExport.Extensions;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Utils;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Locks;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Message;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Model;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.SessionToken;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Proxy;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.ServiceContract.Model;
using Autodesk.Social.Services.Files.ServiceContracts.Client.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class ExportModels
    {
        private readonly ExportCredential _credential;
        private readonly ModelIdentity _modelIdentity;

        public ExportModels(ExportCredential exportCredential)
        {
            _credential = exportCredential;
            _modelIdentity = GetModelIdentity();
        }

        private ModelIdentity GetModelIdentity()
        {
            var sessionToken = SessionTokenGenerator.CreateServiceSessionToken();
            IClientProxy<IModelService> bufferedProxy = GetBufferedProxy();
            IModelService proxy = bufferedProxy.Proxy;
            return proxy.IdentifyModel(sessionToken, _credential.ModelPath, true);
        }

        public bool Export()
        {
            LockData(out EpisodeGuid creationDate);
            var fileList = GetFileList();

            foreach (var fileName in fileList)
            {
                string sourceFile = GetSourceFileName(fileName);
                string targetFile = Path.Combine(_credential.TempTargetFolder, Path.GetFileName(fileName.ToString()));

                DownloadFile(sourceFile, targetFile, creationDate);
            }
            return CreateRvt();
        }

        private bool CreateRvt()
        {
            var rvtService = new RvtService();
            string centralModelFullPath = _credential.ModelLocation.GetUserVisiblePathFromModelLocation();
            return rvtService.GenerateRvtFileFromModelDataFolder(_credential.TempTargetFolder, DataFormatVersion.Latest, _credential.SavePath, true, true, _modelIdentity, centralModelFullPath);
        }

        public bool DownloadFile(string sourceFile, string targetFile, EpisodeGuid creationDate)
        {
            try
            {
                IClientProxy<IModelService> clientProxy = GetStreamedProxy();
                IModelService proxy = clientProxy.Proxy;

                return PerformDownload(proxy, sourceFile, targetFile, creationDate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool PerformDownload(IModelService proxy, string sourceFile, string targetFile, EpisodeGuid creationDate)
        {
            var sessionToken = CreateServiceModelSessionToken();
            using (FileDownloadMessageStream fileDownloadMessageStream = proxy.DownloadFile(new FileDownloadRequestMessage(sessionToken, creationDate, sourceFile)))
            {
                if (fileDownloadMessageStream.Stream == null)
                {
                    throw new Exception();
                }
                string directoryName = Path.GetDirectoryName(targetFile);
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                DateTime now = DateTime.Now;
                using (FileStream fileStream = File.Open(targetFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] array = new byte[16384];
                    int num = 0;
                    do
                    {
                        num = fileDownloadMessageStream.Stream.Read(array, 0, array.Length);
                        if (num > 0)
                        {
                            fileStream.Write(array, 0, num);
                        }
                    }
                    while (num > 0);
                }
                fileDownloadMessageStream.Stream.Close();
            }
            return true;
        }

        public string GetSourceFileName(string fileName)
        {
            return Path.Combine(_modelIdentity.IdentityGUID.GUID.ToString(), fileName);
        }

        private IEnumerable<string> GetFileList()
        {
            var proxy = GetBufferedProxy().Proxy;
            var sessionToken = CreateServiceModelSessionToken();
            proxy.GetListOfModelDataFilesWithoutLocking(sessionToken, out var arrayList);
            return arrayList.Cast<string>();
        }

        private LockStatus LockData(out EpisodeGuid creationDate)
        {
            var sessionToken = CreateServiceModelSessionToken();

            uint lockOptions = 129u;

            ModelVersion localModelVersion = new ModelVersion(new VersionNumber(0), new ModelHistoryCheckInfo(EpisodeGuid.Invalid));

            IClientProxy<IModelService> bufferedProxy = GetBufferedProxy();
            IModelService proxy = bufferedProxy.Proxy;
            return proxy.LockData(sessionToken, lockOptions, bAllowBecomeNonExclusive: true, localModelVersion, out creationDate);
        }

        private ServiceModelSessionToken CreateServiceModelSessionToken()
        {
            ServiceModelSessionToken serviceModelSessionToken = new ServiceModelSessionToken(_modelIdentity, _credential.UserName, _credential.SsoUserName, Environment.MachineName, Guid.NewGuid().ToString())
            {
                ModelLocation = _credential.ModelLocation
            };
            return serviceModelSessionToken;
        }

        private IClientProxy<IModelService> GetStreamedProxy()
        {
            return ProxyProvider.CreateProxyInstance(_credential.RevitVersion).GetStreamedProxy<IModelService>(_credential.HostIp);
        }

        private IClientProxy<IModelService> GetRoutedProxy()
        {
            return ProxyProvider.CreateProxyInstance(_credential.RevitVersion).GetStreamedProxy<IModelService>(_credential.HostIp);
        }

        private IClientProxy<IModelService> GetBufferedProxy()
        {
            return ProxyProvider.CreateProxyInstance(_credential.RevitVersion).GetStreamedProxy<IModelService>(_credential.HostIp);
        }
    }
}