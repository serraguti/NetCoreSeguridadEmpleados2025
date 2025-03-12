﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NetCoreSeguridadEmpleados.Repositories;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute,
        IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, SOLAMENTE NOS VA A INTERESAR SI 
            //EXISTE EL EMPLEADO
            var user = context.HttpContext.User;
            //NECESITAMOS EL CONTROLLER Y EL ACTION DE DONDE 
            //HA PULSADO EL USUARIO ANTES DE ENTRAR AQUI
            string controller =
                context.RouteData.Values["controller"].ToString();
            string action =
                context.RouteData.Values["action"].ToString();
            ITempDataProvider provider =
                context.HttpContext.RequestServices
                .GetService<ITempDataProvider>();
            //ESTA CLASE TIENE EL TEMPDATA DE NUESTRA APP
            var TempData =
                provider.LoadTempData(context.HttpContext);
            TempData["controller"] = controller;
            TempData["action"] = action;
            //GUARDAMOS EL TEMPDATA QUE ACABAMOS DE RECUPERAR 
            //DENTRO DE LA APLICACION
            provider.SaveTempData(context.HttpContext, TempData);
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRoute("Managed", "Login");
            }
            else
            {
                //COMPROBAMOS LOS ROLES DE ACCESO
                if (user.IsInRole("PRESIDENTE") == false
                    && user.IsInRole("DIRECTOR") == false
                    && user.IsInRole("ANALISTA") == false)
                {
                    context.Result = this.GetRoute("Managed", "ErrorAcceso");
                }
            }
        }

        //TENDREMOS MULTIPLES REDIRECCIONES, POR LO QUE NOS CREAMOS UN 
        //METODO PARA FACILITAR EL CODIGO
        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new {controller = controller, action = action}
                    );
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
