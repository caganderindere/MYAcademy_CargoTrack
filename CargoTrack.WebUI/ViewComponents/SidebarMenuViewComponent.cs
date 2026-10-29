using CargoTrack.WebUI.Consts;
using CargoTrack.WebUI.Models.Navigation;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.ViewComponents
{
    public class SidebarMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var sections = User.IsInRole(Roles.Admin) ? AdminMenu()
                : User.IsInRole(Roles.Manager) ? ManagerMenu()
                : CustomerMenu();

            var path = HttpContext.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
            var best = sections
                .SelectMany(x => x.Items)
                .Where(x => path.StartsWith(x.Url.ToLowerInvariant()))
                .OrderByDescending(x => x.Url.Length)
                .FirstOrDefault();

            if (best is not null)
            {
                best.IsActive = true;
            }

            return View(sections);
        }

        private static List<MenuSection> AdminMenu()
        {
            return new List<MenuSection>
            {
                new() { Title = "Genel", Items = { new("Genel Bakış", "bi-grid-1x2", "/Admin/Dashboard"), new("Performans Raporu", "bi-graph-up-arrow", "/Admin/Report") } },
                new() { Title = "Operasyon", Items = { new("Tüm Kargolar", "bi-box-seam", "/Manager/Cargo/Index"), new("Yeni Gönderi", "bi-plus-square", "/Manager/Cargo/Create"), new("Sorunlu Kargolar", "bi-exclamation-triangle", "/Manager/Cargo/Problems"), new("Müşteriler", "bi-people", "/Manager/Customer") } },
                new() { Title = "Ağ Yönetimi", Items = { new("Şubeler", "bi-shop", "/Admin/Branch"), new("Transfer Merkezleri", "bi-diagram-3", "/Admin/TransferCenter"), new("Personeller", "bi-person-badge", "/Admin/Employee"), new("Şehirler", "bi-map", "/Admin/City") } },
                new() { Title = "Ticari", Items = { new("Fiyat Tarifeleri", "bi-tags", "/Admin/CargoPrice") } },
                new() { Title = "Sistem", Items = { new("Kullanıcılar ve Roller", "bi-shield-lock", "/Admin/User"), new("Audit Log", "bi-journal-text", "/Admin/AuditLog"), new("Kurumsal İçerik", "bi-file-richtext", "/Admin/About") } }
            };
        }

        private static List<MenuSection> ManagerMenu()
        {
            return new List<MenuSection>
            {
                new() { Title = "Genel", Items = { new("Şube Paneli", "bi-grid-1x2", "/Manager/Dashboard"), new("Günlük Operasyon", "bi-calendar2-week", "/Manager/Operation") } },
                new() { Title = "Kargo", Items = { new("Şube Kargoları", "bi-box-seam", "/Manager/Cargo/Index"), new("Kargo Kabul", "bi-plus-square", "/Manager/Cargo/Create"), new("Dağıtım Listesi", "bi-truck", "/Manager/Cargo/Distribution"), new("Sorunlu Kargolar", "bi-exclamation-triangle", "/Manager/Cargo/Problems") } },
                new() { Title = "Müşteri", Items = { new("Müşteriler", "bi-people", "/Manager/Customer") } }
            };
        }

        private static List<MenuSection> CustomerMenu()
        {
            return new List<MenuSection>
            {
                new() { Title = "Hesabım", Items = { new("Özet", "bi-grid-1x2", "/User/Dashboard"), new("Kargolarım", "bi-box-seam", "/User/Cargo"), new("Adreslerim", "bi-geo-alt", "/User/Address") } },
                new() { Title = "Hizmetler", Items = { new("Kargo Takip", "bi-search", "/Tracking") } }
            };
        }
    }
}
