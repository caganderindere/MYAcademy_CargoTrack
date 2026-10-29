using CargoTrack.Business.Services.Dashboards;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class TopBranchesViewComponent(IDashboardService _dashboardService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int take = 5)
        {
            return View(await _dashboardService.GetTopBranchesAsync(take));
        }
    }
}
