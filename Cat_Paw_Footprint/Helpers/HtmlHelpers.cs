using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cat_Paw_Footprint.Helpers
{
    namespace Cat_Paw_Footprint.Helpers
    {
        public static class HtmlHelpers
        {
            public static IHtmlContent ActiveBadge(this IHtmlHelper htmlHelper, bool isActive)
            {
                var badgeClass = isActive ? "badge bg-success" : "badge bg-warning";
                var badgeText = isActive ? "上架" : "下架";

                return new HtmlString($"<span class=\"{badgeClass}\">{badgeText}</span>");
            }
        }
    }
}
