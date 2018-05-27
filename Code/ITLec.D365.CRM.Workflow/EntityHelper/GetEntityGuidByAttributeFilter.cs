using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

using ITLec.D365.CRM.Toolkit;

namespace ITLec.D365.CRM.Workflow.EntityHelper
{
    public class GetEntityGuidByAttributeFilter : CodeActivity
    {

        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }




        [Input("SearchBy-FieldName")]
        [Default("")]
        public InArgument<string> SearchByFieldName { get; set; }


        [Input("SearchBy-FieldValue")]
        [Default("")]
        public InArgument<string> SearchByFieldValue { get; set; }




        [Output("First Retrieved Entity Guid")]
        public OutArgument<string> FirstRetrievedEntityGuid { get; set; }


        [Output("Total Retrieved Records")]
        public OutArgument<int> TotalRecords { get; set; }



        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            try
            {
                IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



                string entityName = EntityName.Get<string>(codeActivityContext);
                string[] searchByFieldNames = SearchByFieldName.Get<string>(codeActivityContext).Split(',');
                string[] searchByFieldValues = SearchByFieldValue.Get<string>(codeActivityContext).Split(',');

                QueryByAttribute query = new QueryByAttribute(entityName);
                query.ColumnSet = new ColumnSet(false);
                query.Attributes.AddRange(searchByFieldNames);
                query.Values.AddRange(searchByFieldValues);

                EntityCollection retrievedEntities = service.RetrieveMultiple(query);

                TotalRecords.Set(codeActivityContext, retrievedEntities.Entities.Count);
                string firstEntityGuid = "";
                if (retrievedEntities != null && retrievedEntities.Entities.Count > 0)
                {
                    firstEntityGuid = retrievedEntities.Entities[0].Id.ToString();
                }

                FirstRetrievedEntityGuid.Set(codeActivityContext, firstEntityGuid);

            }
            catch (Exception exce)
            {
                throw new Exception($@"Source: 
                                                ITLec.D365.CRM.Workflow.Attribute.GetEntityGuidByAttributeFilter
                                       ExceptionMSG:
                                                {exce.Message}
                                       ExceptionToString:
                                                {exce.ToString()}");
            }
        }



    }
}
