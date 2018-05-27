using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Workflow
{
    public class JSONManagerFacade
    {



        public static object GetSdkMSGFiltersByMSGName(IOrganizationService service, string msgName)
        {
            object retVal = null;

            Guid msgGuid = GetSdkMessageGuidByName(service, msgName);

            retVal = GetSdkMessageFilterByMessageGuid(service, msgGuid);

            return retVal;
        }

        public static object GetSdkMessageFilterByMessageGuid(IOrganizationService service, Guid msgGuid)
        {
            object retVal = null;

            string entityName = "sdkmessagefilter";
            string filterFieldName = "sdkmessageid";
            Guid filterFieldValue = msgGuid;
            QueryByAttribute query = new QueryByAttribute(entityName);
            query.ColumnSet = new ColumnSet(true);
            query.Attributes.AddRange(filterFieldName);
            query.Values.AddRange(filterFieldValue);
            EntityCollection retrievedEntities = service.RetrieveMultiple(query);
            if (retrievedEntities != null && retrievedEntities.Entities.Count > 0)
            {
                retVal = retrievedEntities.Entities;// JsonConvert.SerializeObject(retrievedEntities.Entities);
            }
            return retVal;
        }

        public static Guid GetSdkMessageGuidByName(IOrganizationService service, string msgName)
        {

            Guid retVal = Guid.Empty;
            string entityName = "sdkmessage";
            string filterFieldName = "name";
            string filterFieldValue = msgName;


            QueryByAttribute query = new QueryByAttribute(entityName);
            query.ColumnSet = new ColumnSet(false);
            query.Attributes.AddRange(filterFieldName);
            query.Values.AddRange(filterFieldValue);

            EntityCollection retrievedEntities = service.RetrieveMultiple(query);
            if (retrievedEntities != null && retrievedEntities.Entities.Count > 0)
            {
                retVal = retrievedEntities.Entities[0].Id;
            }

            return retVal;
        }
    }
}
