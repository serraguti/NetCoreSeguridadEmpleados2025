using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryHospital repo;

        public ManagedController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> 
            Login(string username, string password)
        {
            int idEmpleado = int.Parse(password);
            Empleado empleado = await
                this.repo.LogInEmpleadoAsync(username, idEmpleado);
            if (empleado != null)
            {
                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);
                Claim claimName =
                    new Claim(ClaimTypes.Name, empleado.Apellido);
                identity.AddClaim(claimName);
                //ALMACENAMOS EL ID
                Claim claimId =
                    new Claim(ClaimTypes.NameIdentifier
                    , empleado.IdEmpleado.ToString());
                identity.AddClaim(claimId);
                //COMO ROLE, VOY A UTILIZAR EL DATO DEL OFICIO
                Claim claimOficio =
                    new Claim(ClaimTypes.Role, empleado.Oficio);
                identity.AddClaim(claimOficio);
                Claim claimSalario =
                    new Claim("Salario", empleado.Salario.ToString());
                identity.AddClaim(claimSalario);
                Claim claimDept =
                    new Claim("Departamento", empleado.Departamento.ToString());
                identity.AddClaim(claimDept);
                //INCLUIMOS UN CLAIM DE ADMIN A CUALQUIER EMPLEADO 
                //AL AZAR (ARROYO)
                if (empleado.IdEmpleado == 7499)
                {
                    //CREAMOS UN CLAIM
                    Claim claimAdmin =
                        new Claim("Admin", "Soy el super jefe de la empresa");
                    identity.AddClaim(claimAdmin);
                }

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal);
                string controller =
                    TempData["controller"].ToString();
                string action =
                    TempData["action"].ToString();
                if (TempData["id"] != null)
                {
                    string id =
                        TempData["id"].ToString();
                    return RedirectToAction(action, controller
                        , new { id = id });
                }
                else
                {
                    return RedirectToAction(action, controller);
                }
            }
            else
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
