using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

using ITLec.D365.CRM.Toolkit;

namespace ITLec.D365.CRM.Workflow.Metadata
{
  public  class GetAttributeMetadata : CodeActivity
    {


        [RequiredArgument]
        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }


        [RequiredArgument]
        [Input("AttributeName")]
        [Default("")]
        public InArgument<string> AttributeName { get; set; }



        [Output("AttributeDisplayName")]
        public OutArgument<string> AttributeDisplayName { get; set; }

        [Output("AttributeDescription")]
        public OutArgument<string> AttributeDescription { get; set; }

        //ApplicationRequired=2, None=0, Recommended=3, SystemRequired=
        [Output("AttributeRequiredLevel")]
        public OutArgument<string> AttributeRequiredLevel { get; set; }
        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            try
            {
                Microsoft.Xrm.Sdk.Workflow.IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                MetadataService metadataService = new MetadataService(service);


                string entityName = EntityName.Get<string>(codeActivityContext);
                string attributeName = AttributeName.Get<string>(codeActivityContext);

                var attribute =metadataService.RetrieveAttribute(entityName, attributeName);

                AttributeDisplayName.Set(codeActivityContext, attribute.DisplayName.UserLocalizedLabel.Label);
                AttributeDescription.Set(codeActivityContext, attribute.Description.UserLocalizedLabel.Label);

                AttributeRequiredLevel.Set(codeActivityContext, attribute.RequiredLevel.Value.ToString());

            }
            catch (Exception exce)
            {
                throw new Exception($@"Source: 
                                                ITLec.D365.CRM.Workflow.LookupHelper
                                       ExceptionMSG:
                                                {exce.Message}
                                       ExceptionToString:
                                                {exce.ToString()}");
            }
        }
    }
}
