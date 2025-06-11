using System.Web.Mvc;

namespace Hinet.Web.Areas.QLNhanSuChinhThucArea
{
    public class QLNhanSuChinhThucAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QLNhanSuChinhThucArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QLNhanSuChinhThucArea_default",
                "QLNhanSuChinhThucArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}