using CargoTrack.Business.Services.Dashboards;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class RecentCargosViewComponent(IDashboardService _dashboardService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int take = 6, string detailUrl = "/Manager/Cargo/Detail/")
        {
            ViewData["DetailUrl"] = detailUrl;
            return View(await _dashboardService.GetRecentCargosAsync(take));
        }
    }
}
