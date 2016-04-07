using System.Web;
using System.Web.Mvc;

namespace TrackableWebApiPatterns1.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
