

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Web;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace ITLec.D365.CRM.Workflow
{


    public class DynamicURLHelper : CodeActivity
    {


        [Input("DynamicURL")]
        [Default("")]
        public InArgument<string> DynamicURL { get; set; }

        [Output("EntityID")]
        public OutArgument<string> EntityID { get; set; }

        [Output("CrmOrganizationURL")]
        public OutArgument<string> CrmOrganizationURL { get; set; }

        [Output("ObjectTypeCode")]
        public OutArgument<string> ObjectTypeCode { get; set; }

        [Output("EntityName")]
        public OutArgument<string> EntityName { get; set; }


        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            Microsoft.Xrm.Sdk.Workflow.IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



            string dynamicURL = DynamicURL.Get<string>(codeActivityContext);


            Uri myUri = new Uri(dynamicURL);

            string entityId = HttpUtility.ParseQueryString(myUri.Query).Get("id");
            EntityID.Set(codeActivityContext, entityId);

            string organizationURL = dynamicURL.Split(new string[] { "/main.aspx" }, StringSplitOptions.None)[0];
            CrmOrganizationURL.Set(codeActivityContext, organizationURL);


            string objectTypeCodeStr = HttpUtility.ParseQueryString(myUri.Query).Get("etc");

            int objectTypeCode = 0;

            if (int.TryParse(objectTypeCodeStr, out objectTypeCode))
            {
                ObjectTypeCode.Set(codeActivityContext, objectTypeCodeStr);

                RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };

                // Retrieve the MetaData.
                RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)service.Execute(request);

                EntityMetadata currentEntity = response.EntityMetadata.Where(e => e.ObjectTypeCode == objectTypeCode).FirstOrDefault();

                if (currentEntity != null)
                {
                    string entityName = currentEntity.SchemaName;

                    EntityName.Set(codeActivityContext, entityName);
                }
            }

            /*
            MetadataFilterExpression EntityFilter = new MetadataFilterExpression(LogicalOperator.And);
            EntityFilter.Conditions.Add(new MetadataConditionExpression("objecttypecode", MetadataConditionOperator.Equals, objectTypeCode));
            MetadataPropertiesExpression EntityProperties = new MetadataPropertiesExpression()
            {
                AllProperties = true
            };
            // EntityProperties.PropertyNames.AddRange(new string[] { "Attributes" });

            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {

                Criteria = EntityFilter,
                Properties = EntityProperties

            };*/
        }
    }
}