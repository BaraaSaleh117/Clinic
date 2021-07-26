using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Specialization>().HasData(
                new {Id=(long) 1, SpecializationName="Sp1" },
                new {Id= (long) 2, SpecializationName="Sp2"}
            );
          
        }

        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Specialization> Specialization { get; set; }
        public DbSet<MedicalHistory> MedicalHistory { get; set; }
        public DbSet<AppointmentType> AppointmentType { get; set; }
    }
}
