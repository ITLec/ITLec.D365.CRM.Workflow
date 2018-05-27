using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
   public class LabelFacade
    {
        public static string GetLabelString(Microsoft.Xrm.Sdk.Label label)
        {
            string retVal = "";

            if(label != null)
            {
                if(label.UserLocalizedLabel != null)
                {
                    retVal = (string.IsNullOrEmpty(label.UserLocalizedLabel.Label)) ? "" : label.UserLocalizedLabel.Label;
                }
            }

            return retVal;
        }
    }
}
