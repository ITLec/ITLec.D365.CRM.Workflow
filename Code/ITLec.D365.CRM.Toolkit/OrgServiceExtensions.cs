using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITLec.D365.CRM.Toolkit
{

 public static class OrgServiceExtensions
    {
    /*      [Serializable]
        private sealed class <>c__5<T> where T : Entity, new()
		{
			public static readonly OrgServiceExtensions.<>c__5<T> <>9 = new OrgServiceExtensions.<>c__5<T>();

			public static Func<Entity, T> <>9__5_0;

			internal T<RetrieveMultiple> b__5_0(Entity e)
        {
            return e.ToEntity<T>();
        }
    }*/
/*
    public static T Retrieve<T>(this IOrganizationService service, EntityReference er, params string[] attributeNames) where T : Entity, new()
    {
        return service.Retrieve(er, new ColumnSet(attributeNames));
    }*/

    public static T Retrieve<T>(this IOrganizationService service, EntityReference er, ColumnSet columns) where T : Entity, new()
    {
        Entity entity = service.Retrieve(er.LogicalName, er.Id, columns);
        if (entity != null)
        {
            return entity.ToEntity<T>();
        }
        return Activator.CreateInstance<T>();
    }
        /*
    public static T Retrieve<T>(this IOrganizationService service, string logicalName, Guid id, params string[] attributeNames) where T : Entity, new()
    {
        return service.Retrieve(logicalName, id, new ColumnSet(attributeNames));
    }*/

    public static T Retrieve<T>(this IOrganizationService service, string logicalName, Guid id, ColumnSet columns) where T : Entity, new()
    {
        Entity entity = service.Retrieve(logicalName, id, columns);
        if (entity != null)
        {
            return entity.ToEntity<T>();
        }
        return Activator.CreateInstance<T>();
    }

    public static Entity Retrieve(this IOrganizationService service, EntityReference er, ColumnSet columns)
    {
        if (er == null)
        {
            return null;
        }
        return service.Retrieve(er.LogicalName, er.Id, columns);
    }

 /*   public static IEnumerable<T> RetrieveMultiple<T>(this IOrganizationService service, QueryBase query) where T : Entity, new()
    {
        EntityCollection entityCollection = service.RetrieveMultiple(query);
        if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
        {
            IEnumerable<Entity> arg_52_0 = entityCollection.Entities;
            Func<Entity, T> arg_52_1;
            if ((arg_52_1 = OrgServiceExtensions.<> c__5<T>.<> 9__5_0) == null)
            {
                arg_52_1 = (OrgServiceExtensions.<> c__5<T>.<> 9__5_0 = new Func<Entity, T>(OrgServiceExtensions.<> c__5<T>.<> 9.< RetrieveMultiple > b__5_0));
            }
            return arg_52_0.Select(arg_52_1);
        }
        return Enumerable.Empty<T>();
    }*/

    public static EntityCollection RetrieveMultipleAll(this IOrganizationService orgService, string fetchXml)
    {
        return orgService.RetrieveMultipleAll(fetchXml, 1, null, 5000);
    }

    public static EntityCollection RetrieveMultipleAll(this IOrganizationService orgService, string fetchXml, int pageNumber, string pagingCookie, int count)
    {
        fetchXml = OrgServiceExtensions.AddPagingAttributes(fetchXml, pagingCookie, pageNumber, count);
        RetrieveMultipleRequest retrieveMultipleRequest = new RetrieveMultipleRequest();
        EntityCollection entityCollection = new EntityCollection();
        bool moreRecords;
        do
        {
            retrieveMultipleRequest.Query = new FetchExpression(fetchXml);
            EntityCollection entityCollection2 = ((RetrieveMultipleResponse)orgService.Execute(retrieveMultipleRequest)).EntityCollection;
            entityCollection.Entities.AddRange(entityCollection2.Entities);
            moreRecords = entityCollection2.MoreRecords;
            pageNumber++;
            fetchXml = OrgServiceExtensions.AddPagingAttributes(fetchXml, entityCollection2.PagingCookie, pageNumber, count);
        }
        while (moreRecords);
        return entityCollection;
    }

    private static string AddPagingAttributes(string fetchXml, string cookie, int page, int count)
    {
        XElement xElement = XDocument.Parse(fetchXml).Element("fetch");
        xElement.SetAttributeValue("page", page);
        xElement.SetAttributeValue("count", count);
        xElement.SetAttributeValue("paging-cookie", cookie);
        return xElement.Document.Root.ToString();
    }

    public static EntityCollection RetrieveMultipleAll(this IOrganizationService orgService, QueryExpression query)
    {
        new RetrieveMultipleRequest().Query = query;
        EntityCollection entityCollection = new EntityCollection();
        query.PageInfo.Count = 5000;
        query.PageInfo.PageNumber = 1;
        bool moreRecords;
        do
        {
            EntityCollection entityCollection2 = orgService.RetrieveMultiple(query);
            entityCollection.Entities.AddRange(entityCollection2.Entities);
            moreRecords = entityCollection2.MoreRecords;
            PagingInfo expr_55 = query.PageInfo;
            int pageNumber = expr_55.PageNumber;
            expr_55.PageNumber = pageNumber + 1;
            query.PageInfo.PagingCookie = entityCollection2.PagingCookie;
        }
        while (moreRecords);
        return entityCollection;
    }

    public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService service, IEnumerable<OrganizationRequest> requestCollection)
    {
        return service.ExecuteMultiple(requestCollection, true, true);
    }

    public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService service, IEnumerable<OrganizationRequest> requestCollection, bool continueOnError, bool returnResponses)
    {
        if (requestCollection.Count<OrganizationRequest>() > 1000)
        {
            throw new ArgumentException("requestCollection must be 1000 requests or less");
        }
        ExecuteMultipleSettings requestSettings = new ExecuteMultipleSettings
        {
            ContinueOnError = continueOnError,
            ReturnResponses = returnResponses
        };
        return service.ExecuteMultiple(requestCollection, requestSettings);
    }

    public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService service, IEnumerable<OrganizationRequest> requestCollection, ExecuteMultipleSettings requestSettings)
    {
        if (requestCollection.Count<OrganizationRequest>() > 1000)
        {
            throw new ArgumentException("requestCollection must be 1000 requests or less");
        }
        OrganizationRequestCollection organizationRequestCollection = new OrganizationRequestCollection();
        organizationRequestCollection.AddRange(requestCollection);
        ExecuteMultipleRequest request = new ExecuteMultipleRequest
        {
            Settings = requestSettings,
            Requests = organizationRequestCollection
        };
        return (ExecuteMultipleResponse)service.Execute(request);
    }

    public static IEnumerable<ExecuteMultipleResponse> ExecuteMultipleAll(this IOrganizationService service, IEnumerable<OrganizationRequest> requestCollection)
    {
        List<ExecuteMultipleResponse> list = new List<ExecuteMultipleResponse>();
        foreach (IEnumerable<OrganizationRequest> current in requestCollection.Batch(1000))
        {
            ExecuteMultipleResponse item = service.ExecuteMultiple(current, true, true);
            list.Add(item);
        }
        return list;
    }
}

}
