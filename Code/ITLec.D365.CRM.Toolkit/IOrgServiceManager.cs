using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
    public interface IOrgServiceManager
    {
        IOrganizationService GetOrgService();

        IOrganizationService GetOrgService(Guid userId);
    }
}
