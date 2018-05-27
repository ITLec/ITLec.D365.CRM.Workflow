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
   public class CopyN2NFromEntity2Another : CodeActivity
    {
        [Input("ToBeCopiedEntityName")]
        [Default("")]
        public InArgument<string> ToBeCopiedEntityName { get; set; }


        [Input("SourceEntityName")]
        [Default("")]
        public InArgument<string> SourceEntityName { get; set; }
        [Input("SourceEntityId")]
        [Default("")]
        public InArgument<string> SourceEntityId { get; set; }


        [Input("SourceIntersectEntityName")]
        [Default("")]
        public InArgument<string> SourceIntersectEntityName { get; set; }
        

        [Input("TargetEntityName")]
        [Default("")]
        public InArgument<string> TargetEntityName { get; set; }


        [Input("TargetEntityId")]
        [Default("")]
        public InArgument<string> TargetEntityId { get; set; }


        [Input("TargetRelationshipName")]
        [Default("")]
        public InArgument<string> TargetRelationshipName { get; set; }


        protected override void Execute(CodeActivityContext codeActivityContext)
        {
            try
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



                string entityToBeCopy = ToBeCopiedEntityName.Get<string>(codeActivityContext); //"itlec_entity2";
                string toBeCopiedEntityIdFieldName = entityToBeCopy + "id";// ToBeCopiedEntityIdFieldName.Get<string>(codeActivityContext);// "ToBeCopiedEntityIdFieldName";

                string sourceEntity = SourceEntityName.Get<string>(codeActivityContext); //"itlec_entity1";
                string sourceEntityIdFieldName = sourceEntity + "id"; // "itlec_entity1id";
                string sourceIntersectEntity = SourceIntersectEntityName.Get<string>(codeActivityContext); //"itlec_itlec_entity2_itlec_entity1";

                string targetRelationshipName = TargetRelationshipName.Get<string>(codeActivityContext); // "itlec_itlec_entity3_itlec_entity2";
                string targetEntityName = TargetEntityName.Get<string>(codeActivityContext); //"itlec_entity3";
                                                                                             //   Guid targetEntityId = context.PrimaryEntityId;

                Guid targetEntityId = Guid.Parse(TargetEntityId.Get<string>(codeActivityContext)); //itlec_entity3.Get(codeActivityContext).Id;//Guid.Parse("04D258C6-4842-E811-81A1-00155DDF8608");// 




                QueryExpression query = new QueryExpression(entityToBeCopy);

                LinkEntity linkEntity1 = new LinkEntity(entityToBeCopy, sourceIntersectEntity, toBeCopiedEntityIdFieldName, toBeCopiedEntityIdFieldName, JoinOperator.Inner);
                LinkEntity linkEntity2 = new LinkEntity(sourceIntersectEntity, sourceEntity, sourceEntityIdFieldName, sourceEntityIdFieldName, JoinOperator.Inner);

                linkEntity1.LinkEntities.Add(linkEntity2);
                ///
                linkEntity1.LinkEntities[0].LinkCriteria = new FilterExpression();

                linkEntity1.LinkEntities[0].LinkCriteria.FilterOperator = LogicalOperator.And;
                linkEntity1.LinkEntities[0].LinkCriteria.Conditions.Add
                (
                    new ConditionExpression(sourceEntityIdFieldName, ConditionOperator.Equal, Guid.Parse(SourceEntityId.Get<string>(codeActivityContext)))
                //ADD ANOTHER CONDITION HERE

                );
                ///

                query.LinkEntities.Add(linkEntity1);

                var relatedEntityCollection = service.RetrieveMultiple(query);

                EntityReferenceCollection relatedEntityReferenceCollection = new EntityReferenceCollection();

                foreach (var relatedEntity in relatedEntityCollection.Entities)
                {

                    relatedEntityReferenceCollection.Add(new EntityReference(relatedEntity.LogicalName, relatedEntity.Id));
                }
                

                // Create an object that defines the relationship between the contact and account.
                Relationship relationship = new Relationship(targetRelationshipName);

                //Associate the contact with the 3 accounts.
                service.Associate(targetEntityName, targetEntityId, relationship, relatedEntityReferenceCollection);
            }
            catch(Exception exce)
            {
                throw new Exception($@"Source: 
                                                ITLec.D365.CRM.Workflow.CopyN2NFromEntity2Another
                                       ExceptionMSG:
                                                {exce.Message}
                                       ExceptionToString:
                                                {exce.ToString()}");
    }
}
    }
}
