using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Periodo_2.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ModalRegistrar()
        {
            return PartialView("Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
}
