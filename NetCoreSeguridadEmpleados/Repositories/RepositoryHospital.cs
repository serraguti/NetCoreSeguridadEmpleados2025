using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Models;
using System.Reflection.Metadata.Ecma335;

namespace NetCoreSeguridadEmpleados.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }

        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            return await this.context.Empleados
                .FirstOrDefaultAsync(x => x.IdEmpleado == idEmpleado);
        }

        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync
            (int idDepartamento)
        {
            return await this.context.Empleados
                .Where(x => x.Departamento == idDepartamento).ToListAsync();
        }

        public async Task UpdateSalarioEmpleadosAsync
            (int idDepartamento, int incremento)
        {
            List<Empleado> empleados = await
                this.GetEmpleadosDepartamentoAsync(idDepartamento);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<Empleado> LogInEmpleadoAsync
            (string apellido, int idEmpleado)
        {
            Empleado empleado = await this.context.Empleados
                .Where(z => z.Apellido == apellido &&
                z.IdEmpleado == idEmpleado).FirstOrDefaultAsync();
            return empleado;
        }
    }
}
