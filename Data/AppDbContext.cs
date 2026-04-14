using Gimnasio.Models;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Asistencias> Asistencias { get; set; }
        public DbSet<Ejercicios> Ejercicios { get; set; }
        public DbSet<Entrenadores> Entrenadores { get; set; }
        public DbSet<Membresias> Membresias { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RutinaEjercicios> RutinaEjercicios { get; set; }
        public DbSet<Rutinas> Rutinas { get; set; }
        public DbSet<SocioMembresia> SocioMembresia { get; set; }
        public DbSet<Socios> Socios { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Users> Users { get; set; }

    }
}