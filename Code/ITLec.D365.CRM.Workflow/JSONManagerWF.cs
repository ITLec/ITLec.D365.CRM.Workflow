
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ITLec.D365.CRM.Workflow
{
    public class JSONManagerWF : CodeActivity
    {
        //<Config><Methods><Method><Name>GetSdkMessageGuidByName</Name><Params><Param><ParamKey>msgName</ParamKey><ParamValue>Update</ParamValue></Param></Params></Method></Methods></Config>
        [Input("XmlConfig")]
        [Default("")]
        public InArgument<string> XmlConfig { get; set; }


        [Input("FetchXmlStr")]
        [Default("")]
        public InArgument<string> FetchXmlStr { get; set; }


        [Output("JSONResult")]
        public OutArgument<string> JSONResult { get; set; }

        [Output("StringResult")]
        public OutArgument<string> StringResult { get; set; }

        protected override void Execute(CodeActivityContext codeActivityContext)
        {

            // Create the tracing service
            ITracingService tracingService = codeActivityContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered JSONManagerWF.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                codeActivityContext.ActivityInstanceId,
                codeActivityContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("JSONManagerWF.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var xmlConfig = XmlConfig.Get(codeActivityContext); 

             var fetchXmlStr = FetchXmlStr.Get(codeActivityContext);

            object objResult = null;


            if (!string.IsNullOrEmpty(fetchXmlStr))
            {
                EntityCollection ec = service.RetrieveMultiple(new FetchExpression(fetchXmlStr));
                objResult = ec.Entities;
            }
            else if (!string.IsNullOrEmpty(xmlConfig))
            {
        //        new LinkEntity()

                   var config = XmlHelper.Deserialize<Config>(xmlConfig);
                List<object> paramList = new List<object>();
                paramList.Add(service);
                if (config != null && config.Methods != null & config.Methods.Count > 0)
                {
                    if (!string.IsNullOrEmpty(config.Methods[0].Name))
                    {
                        var methodName = config.Methods[0].Name;

                        //  if (config.Methods[0].Params != null && config.Methods[0].Params != null & config.Methods[0].Params.Count > 0)
                        if (config.Methods[0].Params != null)
                        {

                            foreach (var param in config.Methods[0].Params)
                            {
                                paramList.Add(param.ParamValue);
                            }

                            objResult = CallMethod(methodName, paramList.ToArray());


                        }
                    }
                }

            }



            string _jsonResult = JsonConvert.SerializeObject(objResult);

            JSONResult.Set(codeActivityContext, _jsonResult);


            StringResult.Set(codeActivityContext, objResult.ToString());
        }


        private object CallMethod(string methodName, object[] parametersArray)
        {

            object result = null;
            //  Assembly assembly = Assembly.LoadFile("...Assembly1.dll");
            Type type = typeof(JSONManagerFacade); // assembly.GetType("TestAssembly.Main");

            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(methodName);

                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);

                    if (parameters.Length == 0)
                    {
                        // This works fine
                        result = methodInfo.Invoke(classInstance, null);
                    }
                    else
                    {

                        // The invoke does NOT work;
                        // it throws "Object does not match target type"             
                        result = methodInfo.Invoke(classInstance, parametersArray);
                    }


                }
            }

            return result;
        }
    }





    public class Config
    {

        public List<Method> Methods { get; set; }
    }
    public class Method
    {
        public string Name { get; set; }

        public List<Param> Params { get; set; }
    }

    public class Param
    {
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
    }
}
