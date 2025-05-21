using App.Domain.Identity;

namespace WebApp.Areas.Admin.ViewModels;

public class PasswordLinkViewModel
{
    public string PasswordLink { get; set; } = default!;
    public AppUser AppUser { get; set; } = default!;
}