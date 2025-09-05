using Microsoft.AspNetCore.Mvc;

namespace cuestionarioNom.Areas.Admin
{
    public class AdminAreaRegistration : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
