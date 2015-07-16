using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_CMS.Models;
using System.Configuration;

namespace MVC_CMS
{
    public class DB
    {
        public static TRACNGHIEMEntities GetContext()
        {
            try
            {
                return new TRACNGHIEMEntities();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
