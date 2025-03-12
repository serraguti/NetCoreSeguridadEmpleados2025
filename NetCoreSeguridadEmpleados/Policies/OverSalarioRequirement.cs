using Microsoft.AspNetCore.Authorization;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class OverSalarioRequirement :
        AuthorizationHandler<OverSalarioRequirement>,
        IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync
            (AuthorizationHandlerContext context
            , OverSalarioRequirement requirement)
        {
            //PODEMOS PREGUNTAR SI EXISTE O NO UN CLAIM
            if (context.User.HasClaim(x => x.Type == "Salario") == false)
            {
                context.Fail();
            }
            else
            {
                string data =
                    context.User.FindFirst("Salario").Value;
                int salario = int.Parse(data);
                if (salario >= 250000)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
