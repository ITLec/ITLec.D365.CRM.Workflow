using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{/*
    class XrmConnectionManager
    {
        public static string GetCRMConnectionString()
        {
            UserCredential
            string crmURL = ConfigurationManager.AppSettings["CRMURL"];
            string domainName = ConfigurationManager.AppSettings["CRMDomain"];
            string userName = ConfigurationManager.AppSettings["CRMUsername"];
            string password = ConfigurationManager.AppSettings["CRMPassword"];
            string crmConnectionString = ("Url=" + crmURL + "; Domain=" + domainName + "; Username=" + userName + "; Password=" + password + ";");
            return crmConnectionString;
        }

        public static XrmServiceContext GetXrmServiceContext(bool isCaching = true)
        {
            XrmServiceContext xrmServiceContext = null;
            if (isCaching)
            {
                CrmConnection con = GetCrmConnection();
                xrmServiceContext = new XrmServiceContext(GetCrmConnection());
            }
            else
            {


                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = ConfigurationManager.AppSettings["CRMUsername"]; //MscrmWebService.LoginName;
                credentials.UserName.Password = ConfigurationManager.AppSettings["CRMPassword"];


                string link = ConfigurationManager.AppSettings["CRMURL"] + "/XRMServices/2011/Organization.svc";
                Uri organizationUri = new Uri(link);
                Uri homeRealmUri = null;

                using (var serviceProxy = new OrganizationServiceProxy(organizationUri, homeRealmUri, credentials, null))
                {
                    serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
                    var service = (IOrganizationService)serviceProxy;
                    xrmServiceContext = new XrmServiceContext(service);
                }
            }

            return xrmServiceContext;
        }
    }*/
}
