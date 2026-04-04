using GimDeportivo.Models;
using Microsoft.EntityFrameworkCore;

namespace GimDeportivo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Registro> Registros { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}