using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Workflow
{
   public class WorkflowHelper : CodeActivity
    {

        [Output("Current WF - Record ID")]
        public OutArgument<string> CurrentWF_RecordId { get; set; }

        [Output("Current WF - Record EntityName")]
        public OutArgument<string> CurrentWF_RecordEntityName { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            Guid entityId = context.PrimaryEntityId;
            CurrentWF_RecordId.Set(executionContext, entityId.ToString());
            CurrentWF_RecordEntityName.Set(executionContext, context.PrimaryEntityName);
        }
    }
}
