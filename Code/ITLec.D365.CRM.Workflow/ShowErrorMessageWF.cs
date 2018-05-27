using System;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace ITLec.D365.CRM.Workflow
{
    public class ShowErrorMessageWF : CodeActivity
    {
        [RequiredArgument]
        [Input("ErrorMessage")]
        [Default("")]
        public InArgument<string> ErrorMessage { get; set; }


        protected override void Execute(CodeActivityContext codeActivityContext)
        {

            string errorMSG = ErrorMessage.Get<string>(codeActivityContext);

            throw new Exception(errorMSG);

        }
    }
}
