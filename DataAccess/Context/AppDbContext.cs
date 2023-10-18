
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;
using DataAccess.Models.Companies;
using DataAccess.Models.Estados;
using DataAccess.Models.Status;
using DataAccess.Models.Country;
using DataAccess.Models.Documents;
using DataAccess.Models.Users;
using DataAccess.Models.Rols;
using DataAccess.Models.Permissions;
using DataAccess.Models.Ministry;
using DataAccess.Models.Module;
using DataAccess.Models.WorkPlace;
using DataAccess.Models.ModuleUserRole;
using DataAccess.Models.Variables;
using DataAccess.Models.TypesPerson;
using DataAccess.Models.TaxRegimes;
using DataAccess.Models.EconomicActivity;
using DataAccess.Models.EconomicSectors;
using Microsoft.EntityFrameworkCore.Metadata;
using DataAccess.Models.OccupationProfession;
using DataAccess.Models.PsychologistsCenterWork;
using DataAccess.Models.Reports;
using DataAccess.Models.ReportsRole;
using DataAccess.Models.OccupationalLicense;
using DataAccess.Models.RepresentativeCompany;
using DataAccess.Models.PsychosocialEvaluation;

namespace DataAccess.Context
{
    public partial class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Empresas>().Property(u => u.IdConsecutivo).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }

        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<Roles> AspNetRoles { get; set; }
        public DbSet<UserRoles> AspNetUserRoles { get; set; }
        public DbSet<Empresas> empresas { get; set; }
        public DbSet<Estados> estados { get; set; }
        public DbSet<TiposEmpresa> tiposEmpresas { get; set; }
        public DbSet<Pais> pais { get; set; }
        public DbSet<TiposDocumento> tiposDocumento { get; set; }
        public DbSet<PermisosXUsuario> permisosXUsuario { get; set; }
        public DbSet<Ministerio> ministerio { get; set; }
        public DbSet<Modulo> modulo { get; set; }
        public DbSet<CentroTrabajo> centroTrabajo { get; set; }
        public DbSet<UserWorkPlace> userWorkPlace { get; set; }
        public DbSet<ModuloUserRole> moduloUserRole { get; set; }
        public DbSet<Variables> variables { get; set; }
        public DbSet<TiposPersona> tiposPersona { get; set; }
        public DbSet<RegimenesTributario> regimenesTributario { get; set; }
        public DbSet<ActividadEconomica> actividadEconomica { get; set; }
        public DbSet<SectoresEconomicos> sectoresEconomicos { get; set; }
        public DbSet<OcupacionProfesion> ocupacionProfesion { get; set; }
        public DbSet<PsicologosCentroTrabajo> psicologosCentroTrabajo { get; set; }
        public DbSet<CentroTrabajoHistorial> centroTrabajoHistorial { get; set; }
        public DbSet<Reportes> reportes { get; set; }
        public DbSet<ReportesRole> reportesRole { get; set; }
        public DbSet<EvaluacionPsicosocialUsuario> evaluacionPsicosocialUsuario { get; set; }
        public DbSet<LicenciaOcupacional> licenciaOcupacional { get; set; }
        public DbSet<RepresentanteEmpresa> representanteEmpresa { get; set; }

        //private void SeeData(ModelBuilder modelBuilder)
        //{
        //    var rolAdminId = "3134ea79-fd86-4f89-9784-cbf6b6b18f82";
        //    var userAdminId = "f1173d7f-eddd-4194-bd76-2c6244b2224c";
        //    var userName = "john.culma@outlook.com";

        //    var rolAdmin = new IdentityRole()
        //    {
        //        Id = rolAdminId,
        //        Name = "Administrador",
        //        NormalizedName = "Administrador"
        //    };
        //    var passwordHasher = new PasswordHasher<IdentityUser>();
        //    var usuarioAdmin = new IdentityUser()
        //    {
        //        Id = userAdminId,
        //        UserName = userName,
        //        NormalizedUserName = userName,
        //        Email = userName,
        //        NormalizedEmail = userName,
        //        PasswordHash = passwordHasher.HashPassword(null, "John.fcl14*")
        //    };
        //    modelBuilder.Entity<IdentityUser>()
        //        .HasData(usuarioAdmin);
        //    modelBuilder.Entity<IdentityRole>()
        //        .HasData(rolAdmin);
        //    modelBuilder.Entity<IdentityUserClaim<string>>()
        //        .HasData(new IdentityUserClaim<string>()
        //        {
        //            Id = 1,
        //            ClaimType = ClaimTypes.Role,
        //            UserId = userAdminId,
        //            ClaimValue = "Administrador"
        //        });
        //}
    }
}
