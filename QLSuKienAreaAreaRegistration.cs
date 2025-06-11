using System.Web.Mvc;

namespace Hinet.Web.Areas.QLSuKienArea
{
    public class QLSuKienAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QLSuKienArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QLSuKienArea_default",
                "QLSuKienArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}