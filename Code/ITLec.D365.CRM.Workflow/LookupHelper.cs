using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace ITLec.D365.CRM.Workflow
{
    public class LookupHelper : CodeActivity
    {


        [RequiredArgument]
        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }

        [Input("EntityGuid")]
        [Default("")]
        public InArgument<string> EntityGuid { get; set; }


        [Input("LookupFieldName")]
        [Default("")]
        public InArgument<string> LookupFieldName { get; set; }

        [Output("LookupID")]
        public OutArgument<string> LookupID { get; set; }

        [Output("LookupEntityName")]
        public OutArgument<string> LookupEntityName { get; set; }

        [Output("LookupTextName")]
        public OutArgument<string> LookupTextName { get; set; }


        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            try
            {
                Microsoft.Xrm.Sdk.Workflow.IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



                string entityName = EntityName.Get<string>(codeActivityContext);
                Guid entityGuid = Guid.Parse(EntityGuid.Get<string>(codeActivityContext));
                string lookupFieldName = LookupFieldName.Get<string>(codeActivityContext);
                string[] columns = new string[] { lookupFieldName };
                ColumnSet columnSet = new ColumnSet(columns);






                Entity entity = service.Retrieve(entityName, entityGuid, columnSet);


                
                EntityReference lookup = entity.Attributes[lookupFieldName] as EntityReference;

                LookupID.Set(codeActivityContext, lookup.Id.ToString());

                LookupEntityName.Set(codeActivityContext, lookup.LogicalName);
                LookupTextName.Set(codeActivityContext, lookup.Name.ToString());
            }
            catch(Exception exce)
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
