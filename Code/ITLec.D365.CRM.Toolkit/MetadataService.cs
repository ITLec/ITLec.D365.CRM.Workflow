using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
    public class MetadataService : IMetadataService
    {
        private IOrganizationService _service;

        public MetadataService(IOrganizationService orgService)
        {
            if (orgService == null)
            {
                throw new ArgumentNullException("orgService");
            }
            this._service = orgService;
        }

        public AttributeMetadata RetrieveAttribute(string entityName, string attributeName)
        {
            RetrieveAttributeRequest request = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName.ToLower(),
                LogicalName = attributeName.ToLower()
            };
            return ((RetrieveAttributeResponse)this._service.Execute(request)).AttributeMetadata;
        }

        public EntityMetadata RetrieveEntity(string entityName, EntityFilters entityFilter)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest
            {
                LogicalName = entityName.ToLower(),
                EntityFilters = entityFilter
            };
            return ((RetrieveEntityResponse)this._service.Execute(request)).EntityMetadata;
        }

        public int GetTypeCodeFromEntityName(string entityName)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest
            {
                LogicalName = entityName.ToLower(),
                EntityFilters = EntityFilters.Entity
            };
            RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)this._service.Execute(request);
            if (retrieveEntityResponse != null && retrieveEntityResponse.EntityMetadata.ObjectTypeCode.HasValue)
            {
                return retrieveEntityResponse.EntityMetadata.ObjectTypeCode.Value;
            }
            throw new Exception("Could not find typecode for entity: " + entityName);
        }

        public string GetEntityNameFromTypeCode(int typeCode)
        {
            return this.GetEntityNameFromTypeCode(typeCode, false);
        }

        public string GetEntityNameFromTypeCode(int typeCode, bool displayName)
        {
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };
            EntityMetadata[] entityMetadata = ((RetrieveAllEntitiesResponse)this._service.Execute(request)).EntityMetadata;
            int i = 0;
            while (i < entityMetadata.Length)
            {
                EntityMetadata entityMetadata2 = entityMetadata[i];
                if (entityMetadata2.ObjectTypeCode.HasValue && entityMetadata2.ObjectTypeCode.Value == typeCode)
                {
                    if (!displayName)
                    {
                        return entityMetadata2.LogicalName;
                    }
                    return entityMetadata2.DisplayName.UserLocalizedLabel.Label;
                }
                else
                {
                    i++;
                }
            }
            throw new ArgumentOutOfRangeException("typeCode", string.Format("No entity found of type code {0}", typeCode));
        }

        public List<OptionMetadata> GetStatusOptionsForEntityByState(string entity, int state)
        {
            EnumAttributeMetadata arg_17_0 = (StatusAttributeMetadata)this.RetrieveAttribute(entity, "statuscode");
            List<OptionMetadata> list = new List<OptionMetadata>();
            foreach (OptionMetadata current in arg_17_0.OptionSet.Options)
            {
                if (current.Value.HasValue && current.Value.Value == state)
                {
                    list.Add(current);
                }
            }
            return list;
        }

        public OptionMetadata GetCorrespondingStateOfStatus(string entity, int status)
        {
            EnumAttributeMetadata arg_23_0 = (StatusAttributeMetadata)this.RetrieveAttribute(entity, "statuscode");
            StateAttributeMetadata stateAttributeMetadata = (StateAttributeMetadata)this.RetrieveAttribute(entity, "statecode");
            foreach (OptionMetadata current in arg_23_0.OptionSet.Options)
            {
                if (current.Value.HasValue && current.Value.Value == status)
                {
                    foreach (OptionMetadata current2 in stateAttributeMetadata.OptionSet.Options)
                    {
                        if (current2.Value.HasValue && current2.Value.Value == current.Value.Value)
                        {
                            return current2;
                        }
                    }
                }
            }
            return null;
        }

        public string GetPrimaryAttributeName(string entityName)
        {
            return this.RetrieveEntity(entityName, EntityFilters.Entity).PrimaryNameAttribute;
        }

        public int GetIntValueFromStatusString(string entityName, string statuscodeName)
        {
            return this.GetIntValueFromPicklistString(entityName, "statuscode", statuscodeName);
        }

        public int GetIntValueFromStateString(string entityName, string statecodeName)
        {
            return this.GetIntValueFromPicklistString(entityName, "statecode", statecodeName);
        }

        public int GetIntValueFromPicklistString(string entityName, string attributeName, string picklistValue)
        {
            AttributeMetadata attributeMetadata = this.RetrieveAttribute(entityName, attributeName);
            Type type = attributeMetadata.GetType();
            OptionMetadataCollection optionMetadataCollection = new OptionMetadataCollection();
            if (type == typeof(PicklistAttributeMetadata))
            {
                optionMetadataCollection = ((PicklistAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            else if (type == typeof(StateAttributeMetadata))
            {
                optionMetadataCollection = ((StateAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            else if (type == typeof(StatusAttributeMetadata))
            {
                optionMetadataCollection = ((StatusAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            foreach (OptionMetadata current in optionMetadataCollection)
            {
                if (current.Value.HasValue && current.Label.UserLocalizedLabel.Label.ToLower() == picklistValue.ToLower())
                {
                    return current.Value.Value;
                }
            }
            return -1;
        }

    public int GetIntValueFromPicklistString(string entityName, string attributeName, string picklistValue, int languageCode)
        {
            throw new Exception("Not Implemented Yet");
            /* 
            AttributeMetadata attributeMetadata = this.RetrieveAttribute(entityName, attributeName);
            Type type = attributeMetadata.GetType();
            OptionMetadataCollection optionMetadataCollection = new OptionMetadataCollection();
            if (type == typeof(PicklistAttributeMetadata))
            {
                optionMetadataCollection = ((PicklistAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            else if (type == typeof(StateAttributeMetadata))
            {
                optionMetadataCollection = ((StateAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            else if (type == typeof(StatusAttributeMetadata))
            {
                optionMetadataCollection = ((StatusAttributeMetadata)attributeMetadata).OptionSet.Options;
            }
            Func < LocalizedLabel, bool> <> 9__0;
            foreach (OptionMetadata current in optionMetadataCollection)
            {
                if (current.Value.HasValue)
                {
                    IEnumerable<LocalizedLabel> arg_102_0 = current.Label.LocalizedLabels;
                    Func<LocalizedLabel, bool> arg_102_1;
                    if ((arg_102_1 = <> 9__0) == null)
                    {
                        arg_102_1 = (<> 9__0 = ((LocalizedLabel x) => x.LanguageCode == languageCode && picklistValue.Equals(x.Label, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    if (arg_102_0.FirstOrDefault(arg_102_1) != null)
                    {
                        return current.Value.Value;
                    }
                }
            }
            return -1;*/
        }

        public string GetStringValueFromPicklistInt(string entityName, string attributeName, int picklistValue)
        {
            PicklistAttributeMetadata picklistAttributeMetadata = this.RetrieveAttribute(entityName, attributeName) as PicklistAttributeMetadata;
            if (picklistAttributeMetadata != null)
            {
                foreach (OptionMetadata current in picklistAttributeMetadata.OptionSet.Options)
                {
                    if (current.Value.HasValue && current.Value.Value == picklistValue)
                    {
                        return current.Label.UserLocalizedLabel.Label;
                    }
                }
            }
            return string.Empty;
        }

        public string GetStringValueFromPicklistInt(string entityName, string attributeName, int picklistValue, int languageCode)
        {
            PicklistAttributeMetadata picklistAttributeMetadata = this.RetrieveAttribute(entityName, attributeName) as PicklistAttributeMetadata;
            if (picklistAttributeMetadata != null)
            {
                OptionMetadata optionMetadata = picklistAttributeMetadata.OptionSet.Options.FirstOrDefault((OptionMetadata x) => x.Value == picklistValue);
                if (optionMetadata != null)
                {
                    return optionMetadata.Label.LocalizedLabels.FirstOrDefault((LocalizedLabel x) => x.LanguageCode == languageCode).Label ?? string.Empty;
                }
            }
            return string.Empty;
        }

        public string GetStringValueFromBooleanValue(string entityName, string attributeName, bool booleanValue)
        {
            BooleanAttributeMetadata booleanAttributeMetadata = this.RetrieveAttribute(entityName, attributeName) as BooleanAttributeMetadata;
            if (booleanAttributeMetadata == null)
            {
                return string.Empty;
            }
            if (!booleanValue)
            {
                return booleanAttributeMetadata.OptionSet.FalseOption.Label.UserLocalizedLabel.Label;
            }
            return booleanAttributeMetadata.OptionSet.TrueOption.Label.UserLocalizedLabel.Label;
        }

        public string GetStringValueFromBooleanValue(string entityName, string attributeName, bool booleanValue, int languageCode)
        {
            BooleanAttributeMetadata booleanAttributeMetadata = this.RetrieveAttribute(entityName, attributeName) as BooleanAttributeMetadata;
            if (booleanAttributeMetadata != null)
            {
                OptionMetadata optionMetadata = booleanValue ? booleanAttributeMetadata.OptionSet.TrueOption : booleanAttributeMetadata.OptionSet.FalseOption;
                if (optionMetadata != null)
                {
                    return optionMetadata.Label.LocalizedLabels.FirstOrDefault((LocalizedLabel x) => x.LanguageCode == languageCode).Label ?? string.Empty;
                }
            }
            return string.Empty;
        }

        public string GetStringValueFromStatusInt(string entityName, string attributeName, int statusCode)
        {
            StatusAttributeMetadata statusAttributeMetadata = this.RetrieveAttribute(entityName, attributeName) as StatusAttributeMetadata;
            if (statusAttributeMetadata != null)
            {
                foreach (OptionMetadata current in statusAttributeMetadata.OptionSet.Options)
                {
                    if (current.Value.HasValue && current.Value.Value == statusCode)
                    {
                        return current.Label.UserLocalizedLabel.Label;
                    }
                }
            }
            return string.Empty;
        }

        public bool DoesAttributeExistOnEntity(string entityName, string attributeName)
        {
            try
            {
                this.RetrieveAttribute(entityName, attributeName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetDisplayNameFromEntity(string entityName)
        {
            EntityMetadata entityMetadata = this.RetrieveEntity(entityName, EntityFilters.Entity);
            if (entityMetadata != null)
            {
                return entityMetadata.DisplayName.UserLocalizedLabel.Label;
            }
            return string.Empty;
        }

        public string GetDisplayNameFromAttribute(string entityName, string attributeName)
        {
            AttributeMetadata attributeMetadata = this.RetrieveAttribute(entityName, attributeName);
            if (attributeMetadata != null)
            {
                return attributeMetadata.DisplayName.UserLocalizedLabel.Label;
            }
            return string.Empty;
        }

        public string GetPrimaryKeyName(string entityName)
        {
            EntityMetadata entityMetadata = this.RetrieveEntity(entityName, EntityFilters.Entity);
            if (entityMetadata != null)
            {
                return entityMetadata.PrimaryIdAttribute;
            }
            return string.Empty;
        }

        public List<OptionMetadata> RetrieveGlobalOptionSet(string globalOptionSetName)
        {
            RetrieveOptionSetRequest request = new RetrieveOptionSetRequest
            {
                Name = globalOptionSetName
            };
            return new List<OptionMetadata>(((OptionSetMetadata)((RetrieveOptionSetResponse)this._service.Execute(request)).OptionSetMetadata).Options.ToArray());
        }
    }

}
