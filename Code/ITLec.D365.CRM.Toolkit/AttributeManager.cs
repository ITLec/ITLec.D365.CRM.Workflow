using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
  public static  class AttributeManager
    {
        public static string GetAttributeLabel(Microsoft.Xrm.Sdk.IOrganizationService service, Microsoft.Xrm.Sdk.Entity entity, string attribute)
        {
            string strLabel = String.Empty;
            Microsoft.Xrm.Sdk.OptionSetValue option = null;

            var attributeRequest = new Microsoft.Xrm.Sdk.Messages.RetrieveAttributeRequest
            {
                EntityLogicalName = entity.LogicalName,
                LogicalName = attribute,
                RetrieveAsIfPublished = true
            };

            Microsoft.Xrm.Sdk.Messages.RetrieveAttributeResponse attributeResponse = (Microsoft.Xrm.Sdk.Messages.RetrieveAttributeResponse)service.Execute(attributeRequest);
            Microsoft.Xrm.Sdk.Metadata.AttributeMetadata attrMetadata = (Microsoft.Xrm.Sdk.Metadata.AttributeMetadata)attributeResponse.AttributeMetadata;
            Microsoft.Xrm.Sdk.Metadata.OptionMetadataCollection optionMeta = null;

            //Console.Write("\tDebug attributeType : " + attrMetadata.AttributeType.ToString() + "\t");

            #region Switch
            switch (attrMetadata.AttributeType.ToString())
            {
                case "Status": //Status
                    optionMeta = ((Microsoft.Xrm.Sdk.Metadata.StatusAttributeMetadata)attrMetadata).OptionSet.Options;
                    break;
                case "Picklist": //Picklist
                    optionMeta = ((Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)attrMetadata).OptionSet.Options;
                    break;
                case "State": //State
                    optionMeta = ((Microsoft.Xrm.Sdk.Metadata.StateAttributeMetadata)attrMetadata).OptionSet.Options;
                    break;
                case "Decimal": //Decimal
                    break;
                case "Enum": //Enum
                    break;
                case "Memo": //Memo
                    break;
                case "Money": //Money
                    break;
                case "Lookup":
                    break;
                case "Integer":
                    break;
                case "Owner":
                    strLabel = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes[attribute]).Name;
                    break;
                case "DateTime": //DateTime
                    strLabel = Convert.ToDateTime(entity.Attributes[attribute].ToString()).ToString();
                    break;
                case "Boolean": //Boolean
                    break;
                case "String": //String
                    strLabel = entity[attribute].ToString();
                    break;
                case "Double": //Double
                    break;
                case "EntityName": //Entity Name
                    break;
                case "Image": //Image, it will return image name.
                    break;
                case "BigInt":
                    break;
                case "ManagedProperty":
                    break;
                case "Uniqueidentifier":
                    break;
                case "Virtual":
                    break;
                default:
                    //TODO: Write Err Exception
                    break;
            }
            #endregion Switch

            //If this attr is OptionSet, Find Label
            if (optionMeta != null)
            {
                option = ((Microsoft.Xrm.Sdk.OptionSetValue)entity.Attributes[attribute]);
                foreach (Microsoft.Xrm.Sdk.Metadata.OptionMetadata metadata in optionMeta)
                {
                    if (metadata.Value == option.Value)
                    {
                        strLabel = metadata.Label.UserLocalizedLabel.Label;
                    }
                }
            }

            return strLabel;
        }

        public static string GetAttributeValue(object attributeValue)
        {
            switch (attributeValue.ToString())

            {

                case "Microsoft.Xrm.Sdk.EntityReference":

                    return ((Microsoft.Xrm.Sdk.EntityReference)attributeValue).LogicalName+ "\\"+ ((Microsoft.Xrm.Sdk.EntityReference)attributeValue).Id.ToString();

                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    return ((Microsoft.Xrm.Sdk.OptionSetValue)attributeValue).Value.ToString();

                case "Microsoft.Xrm.Sdk.Money":
                    return ((Microsoft.Xrm.Sdk.Money)attributeValue).Value.ToString();

                case "Microsoft.Xrm.Sdk.AliasedValue":

                    return GetAttributeValue(((Microsoft.Xrm.Sdk.AliasedValue)attributeValue).Value);
                    
                case "Microsoft.Xrm.Sdk.Label":
                    return ITLec.D365.CRM.Toolkit.LabelFacade.GetLabelString((Microsoft.Xrm.Sdk.Label)attributeValue);
                case "Microsoft.Xrm.Sdk.BooleanManagedProperty":
                    return ((Microsoft.Xrm.Sdk.BooleanManagedProperty)attributeValue).Value.ToString();

            }

            return attributeValue.ToString();
        }
    }
}
