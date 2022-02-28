﻿using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.SessionToken;
using System;

namespace AltecSystems.Revit.ServerExport.Utils
{
    internal static class SessionTokenGenerator
    {
        public static ServiceSessionToken CreateServiceSessionToken()
        {
            return new ServiceSessionToken($"RevitServerTool:{Environment.MachineName}:1", string.Empty, Environment.MachineName, Guid.NewGuid().ToString());
        }
    }
}