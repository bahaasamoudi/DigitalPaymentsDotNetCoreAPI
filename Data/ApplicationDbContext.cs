using DigitalPayments.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalPayments.Data
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().HasMany(p => p.Roles).WithOne().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);


            //Create Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "shop", NormalizedName = "SHOP" },
                new IdentityRole { Name = "user", NormalizedName = "USER" });
            ///Create Users
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Email = "bahaa.test@hotmail.com",
                    UserName = "bahaa.samoudi",
                    FirstName = "Bahaa",
                    LastName = "Samoudi",
                    Country = "Palestine",
                    RegisteredDate = DateTime.Now,
                    Birthdate = DateTime.Now,
                    Gender = 0,
                    IdNumber = "5645435"
                });
        }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Bill> Bills { get; set; }


    }
}
