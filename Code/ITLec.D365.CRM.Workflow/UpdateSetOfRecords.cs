using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

using ITLec.D365.CRM.Toolkit;

namespace ITLec.D365.CRM.Workflow
{
    public class UpdateSetOfRecords : CodeActivity
    {

        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }


       /* [Input("SearchEntityName")]
        [Default("")]
        public InArgument<string> SearchEntityName { get; set; }
        */


        [Input("SearchBy-FieldName")]
        [Default("")]
        public InArgument<string> SearchByFieldName { get; set; }


        [Input("SearchBy-FieldValue")]
        [Default("")]
        public InArgument<string> SearchByFieldValue { get; set; }


     /*   [Input("ToBeUpdated-LookupEntityName")]
        [Default("")]
        public InArgument<string> ToBeUpdatedLookupEntityName { get; set; }
        */

        [Input("ToBeUpdated-FieldName")]
        [Default("")]
        public InArgument<string> ToBeUpdatedFieldName { get; set; }


        [Input("ToBeUpdated-FieldValue")]
        [Default("")]
        public InArgument<string> ToBeUpdatedFieldValue { get; set; }



        [Output("Total Records")]
        public OutArgument<int> TotalRecords { get; set; }

        [Output("Total Updated Records")]
        public OutArgument<int> TotalUpdatedRecords { get; set; }


        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            string entityName = "";
            string searchByFieldName = "";
            string searchByFieldValue = "";
            string toBeUpdatedFieldName = "";
            string toBeUpdatedFieldValue = "";
            object toBeUpdatedFieldValueObj = null;
            int totalUpdatedRecords = 0;
            int totalMatchCriteriaRecords = 0;
            try
            {
                IWorkflowContext context = codeActivityContext.GetExtension<IWorkflowContext>();
                IOrganizationServiceFactory serviceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



                entityName = EntityName.Get<string>(codeActivityContext);
                //   string searchEntityName = SearchEntityName.Get<string>(codeActivityContext);
                searchByFieldName = SearchByFieldName.Get<string>(codeActivityContext);
                string[] searchByFieldNames = searchByFieldName.Split(',');
                searchByFieldValue = SearchByFieldValue.Get<string>(codeActivityContext);
                string[] searchByFieldValues = searchByFieldValue.Split(',');
                //      string toBeUpdatedLookupEntityName = ToBeUpdatedLookupEntityName.Get<string>(codeActivityContext);
                 toBeUpdatedFieldName = ToBeUpdatedFieldName.Get<string>(codeActivityContext);
                 toBeUpdatedFieldValue = ToBeUpdatedFieldValue.Get<string>(codeActivityContext);

                QueryByAttribute query = new QueryByAttribute(entityName);
                query.ColumnSet = new ColumnSet(toBeUpdatedFieldName);
                query.Attributes.AddRange(searchByFieldNames);
                query.Values.AddRange(searchByFieldValues);

                EntityCollection retrievedEntities = service.RetrieveMultiple(query);

                 toBeUpdatedFieldValueObj = GetUpdatedValue(service, entityName, toBeUpdatedFieldName, toBeUpdatedFieldValue);


                totalMatchCriteriaRecords = retrievedEntities.Entities.Count;
                TotalRecords.Set(codeActivityContext, totalMatchCriteriaRecords );
                if (retrievedEntities != null && retrievedEntities.Entities.Count > 0)
                {
                    foreach (Entity entity in retrievedEntities.Entities)
                    {
                        if (entity != null)
                        {

                            if (!entity.Attributes.Contains(toBeUpdatedFieldName) || entity.Attributes[toBeUpdatedFieldName] != toBeUpdatedFieldValueObj)
                            {
                                if (entity.Attributes.Contains(toBeUpdatedFieldName))
                                {
                                    if (entity.Attributes[toBeUpdatedFieldName] != toBeUpdatedFieldValueObj)
                                    {
                                        entity.Attributes[toBeUpdatedFieldName] = toBeUpdatedFieldValueObj;
                                    }
                                }
                                else
                                {
                                    entity.Attributes.Add(toBeUpdatedFieldName, toBeUpdatedFieldValueObj);
                                }
                                service.Update(entity);
                                totalUpdatedRecords = totalUpdatedRecords + 1;
                            }
                        }
                    }
                }

                TotalUpdatedRecords.Set(codeActivityContext, totalUpdatedRecords);

            }
            catch (Exception exce)
            {
                throw new Exception($@"Source: 
                                                ITLec.D365.CRM.Workflow.UpdateSetOfRecords
                                       TotalMatchCriteriaRecords:
                                                {totalMatchCriteriaRecords}
                                       TotalUpdatedRecords:
                                                {totalUpdatedRecords}
                                       EntityName:
                                                {entityName}, 
                                       SearchBy
                                                FieldName: {searchByFieldName}
                                                FieldValue: {searchByFieldValue}
                                       ToBeUpdated
                                                FieldName: {toBeUpdatedFieldName}
                                                FieldValue: {toBeUpdatedFieldValue}
                                                FieldValueObj: {toBeUpdatedFieldValueObj}
                                       ExceptionMSG:
                                                {exce.Message}
                                       ExceptionToString:
                                                {exce.ToString()}");
            }
        }

        //TODO
        object GetUpdatedValue(IOrganizationService service,string entityName,string fieldName,string value)
        {
            MetadataService metaDataService = new MetadataService(service);
            object retValue=null;

            var attribute = metaDataService.RetrieveAttribute(entityName, fieldName);

            if(attribute.AttributeType == Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Lookup)
            {

                retValue  = new EntityReference(attribute.LogicalName, Guid.Parse( value.ToString()));
            }
            else
            {
                //TODO:Need More test

                retValue = value;

            }

            return retValue;
        }
    }
}
