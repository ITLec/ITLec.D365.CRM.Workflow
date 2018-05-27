using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

using ITLec.D365.CRM.Toolkit;
using Microsoft.Crm.Sdk.Messages;

namespace ITLec.D365.CRM.Workflow.WF
{

   public class ExecuteWorkflowForSetOfRecords : CodeActivity
    {

        //[RequiredArgument]
        //[Output("Workflow to execute_")]
        //[ReferenceTarget("workflow")]
        //public OutArgument<EntityReference> Workflow_
        //{
        //    get;
        //    set;
        //}

        [RequiredArgument]
        [Input("Workflow to execute")]
        [ReferenceTarget("workflow")]
        public InArgument<EntityReference> Workflow
        {
            get;
            set;
        }


        [RequiredArgument]
        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }




        [RequiredArgument]
        [Input("SearchBy-FieldName")]
        [Default("")]
        public InArgument<string> SearchByFieldName { get; set; }


        [RequiredArgument]
        [Input("SearchBy-FieldValue")]
        [Default("")]
        public InArgument<string> SearchByFieldValue { get; set; }


        [Output("Total Records")]
        public OutArgument<int> TotalRecords { get; set; }



        protected override void Execute(CodeActivityContext codeActivityContext)
        {
                string entityName = EntityName.Get<string>(codeActivityContext);
            string searchByFieldName = SearchByFieldName.Get<string>(codeActivityContext);
            string[] searchByFieldNames = searchByFieldName.Split(',');
            string searchByFieldValue = SearchByFieldValue.Get<string>(codeActivityContext);
            string[] searchByFieldValues = searchByFieldValue.Split(',');
            EntityReference workflowEntityReference = null;
            try
            {
                IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);





                 workflowEntityReference = Workflow.Get<EntityReference>(codeActivityContext);



                QueryByAttribute query = new QueryByAttribute(entityName);
                query.ColumnSet = new ColumnSet(false);
                query.Attributes.AddRange(searchByFieldNames);
                query.Values.AddRange(searchByFieldValues);

                EntityCollection retrievedEntities = service.RetrieveMultiple(query);

                TotalRecords.Set(codeActivityContext, retrievedEntities.Entities.Count);

                if (retrievedEntities != null && retrievedEntities.Entities.Count > 0)
                {

                    foreach (Entity entity in retrievedEntities.Entities)

                    {

                        // Create an ExecuteWorkflow request.

                        ExecuteWorkflowRequest request = new ExecuteWorkflowRequest()



                        {

                            WorkflowId = workflowEntityReference.Id,

                            EntityId = entity.Id

                        };

                        ExecuteWorkflowResponse response =
                        (ExecuteWorkflowResponse)service.Execute(request);

                    }

                }
            }
            catch (Exception exce)
            {
                throw new Exception($@"Source: 
                                                ITLec.D365.CRM.Workflow.WF.ExecuteWorkflowForSetOfRecords
                                       EntityName:
                                                {entityName}
                                       SearchBy-FieldName:
                                                {searchByFieldName}
                                       SearchBy-FieldValue:
                                                {searchByFieldValue}
                                       Workflow Name: {workflowEntityReference.Name}, WorkflowId: {workflowEntityReference.Id}
                                       ExceptionMSG:
                                                {exce.Message}
                                       ExceptionToString:
                                                {exce.ToString()}");
            }
        }



    }
}
