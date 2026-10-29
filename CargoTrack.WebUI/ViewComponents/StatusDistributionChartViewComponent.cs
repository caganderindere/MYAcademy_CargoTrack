using CargoTrack.WebUI.Extensions;
using CargoTrack.WebUI.Models;
using CargoTrack.Business.Services.Dashboards;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class StatusDistributionChartViewComponent(IDashboardService _dashboardService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var rows = await _dashboardService.GetStatusDistributionAsync();

            return View(new ChartViewModel
            {
                Id = "chart-status",
                Type = "doughnut",
                Labels = rows.Select(x => x.Status.GetDisplayName()).ToList(),
                Values = rows.Select(x => x.Count).ToList(),
                Keys = rows.Select(x => (int)x.Status).ToList()
            });
        }
    }
}
