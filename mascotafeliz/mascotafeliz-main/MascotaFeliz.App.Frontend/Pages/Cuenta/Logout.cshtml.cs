using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MascotaFeliz.App.Frontend.Pages.Cuenta
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Dominio.ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<Dominio.ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Cuenta/Login");
        }
    }
}
