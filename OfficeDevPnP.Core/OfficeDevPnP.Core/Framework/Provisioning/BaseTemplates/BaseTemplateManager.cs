﻿using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This class will be used to provide access to the right base template configuration
    /// </summary>
    public static class BaseTemplateManager
    {

        public static ProvisioningTemplate GetBaseTemplate(this Web web)
        {
            web.Context.Load(web, p => p.WebTemplate, p => p.Configuration);
            web.Context.ExecuteQueryRetry();

            ProvisioningTemplate provisioningTemplate = null;

            try
            {
                string baseTemplate = string.Format("OfficeDevPnP.Core.Framework.Provisioning.BaseTemplates.v{0}.{1}{2}Template.xml", GetSharePointVersion(), web.WebTemplate, web.Configuration);
                using (Stream stream = typeof(BaseTemplateManager).Assembly.GetManifestResourceStream(baseTemplate))
                {
                    // Get the XML document from the stream
                    XDocument doc = XDocument.Load(stream);

                    // And convert it into a ProvisioningTemplate
                    provisioningTemplate = XMLSerializer.Deserialize<SharePointProvisioningTemplate>(doc).ToProvisioningTemplate();
                }
            }
            catch(Exception ex)
            {
                //TODO: log message
            }

            return provisioningTemplate;
        }

        private static int GetSharePointVersion()
        {
            Assembly asm = Assembly.GetAssembly(typeof(Microsoft.SharePoint.Client.Site));
            return asm.GetName().Version.Major;
        }

    }
}
