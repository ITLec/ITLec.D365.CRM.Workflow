using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
    public interface IMetadataService
    {
        bool DoesAttributeExistOnEntity(string entityName, string attributeName);

        OptionMetadata GetCorrespondingStateOfStatus(string entity, int status);

        string GetDisplayNameFromAttribute(string entityName, string attributeName);

        string GetDisplayNameFromEntity(string entityName);

        string GetEntityNameFromTypeCode(int typeCode);

        string GetEntityNameFromTypeCode(int typeCode, bool displayName);

        int GetIntValueFromPicklistString(string entityName, string attributeName, string picklistValue);

        int GetIntValueFromPicklistString(string entityName, string attributeName, string picklistValue, int languageCode);

        int GetIntValueFromStateString(string entityName, string statecodeName);

        int GetIntValueFromStatusString(string entityName, string statuscodeName);

        string GetPrimaryAttributeName(string entityName);

        string GetPrimaryKeyName(string entityName);

        List<OptionMetadata> GetStatusOptionsForEntityByState(string entity, int state);

        string GetStringValueFromBooleanValue(string entityName, string attributeName, bool booleanValue);

        string GetStringValueFromBooleanValue(string entityName, string attributeName, bool booleanValue, int languageCode);

        string GetStringValueFromPicklistInt(string entityName, string attributeName, int picklistValue, int languageCode);

        string GetStringValueFromPicklistInt(string entityName, string attributeName, int picklistValue);

        string GetStringValueFromStatusInt(string entityName, string attributeName, int statusCode);

        int GetTypeCodeFromEntityName(string entityName);

        AttributeMetadata RetrieveAttribute(string entityName, string attributeName);

        EntityMetadata RetrieveEntity(string entityName, EntityFilters entityFilter);

        List<OptionMetadata> RetrieveGlobalOptionSet(string globalOptionSetName);
    }
}
