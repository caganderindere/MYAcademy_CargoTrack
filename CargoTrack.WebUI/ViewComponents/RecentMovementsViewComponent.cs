using CargoTrack.Business.Services.Dashboards;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class RecentMovementsViewComponent(IDashboardService _dashboardService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int take = 8)
        {
            return View(await _dashboardService.GetRecentMovementsAsync(take));
        }
    }
}
