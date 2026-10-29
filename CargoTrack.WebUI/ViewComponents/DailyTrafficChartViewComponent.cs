using CargoTrack.WebUI.Models;
using CargoTrack.Business.Services.Dashboards;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class DailyTrafficChartViewComponent(IDashboardService _dashboardService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int days = 7)
        {
            var rows = await _dashboardService.GetDailyTrafficAsync(days);

            return View(new ChartViewModel
            {
                Id = "chart-traffic",
                Type = "line",
                Label = "Gönderi",
                Labels = rows.Select(x => x.Date.ToString("dd MMM")).ToList(),
                Values = rows.Select(x => x.Count).ToList()
            });
        }
    }
}
