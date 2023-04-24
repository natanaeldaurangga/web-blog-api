using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnJwtAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnJwtAuth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppUser>? Users { get; set; }

        public DbSet<Role>? Roles { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<BlogTag> BlogTags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();

            modelBuilder.Entity<Blog>()
            .HasMany(b => b.Comments)
            .WithOne(c => c.Blog)
            .HasForeignKey(c => c.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blog>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Blogs)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            //FIXME: waktu dotnet ef migration Add Init, kita mendapatkan pesan ini, Unable to create an object of type 'AppDbContext'. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728

            modelBuilder.Entity<BlogTag>()
            .HasKey(bt => new { bt.BlogId, bt.TagId });

            modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.Blog)
            .WithMany(b => b.BlogTags)
            .HasForeignKey(bt => bt.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.Tag)
            .WithMany(t => t.BlogTags)
            .HasForeignKey(bt => bt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

            // SeedDatabase(modelBuilder);
        }

        private void SeedDatabase(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
            .HasData(
                new Role { Name = "ROLE_ADMIN" },
                new Role { Name = "ROLE_USER" }
            );

            // CREATING USER
            var adminRole = Roles!.FirstOrDefault(r => r.Name!.Equals("ROLE_ADMIN"));
            const string password = "Test1234";
            using var hmac = new HMACSHA256();
            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            modelBuilder.Entity<AppUser>()
            .HasData(
                new AppUser
                {
                    Name = "Natanael Daurangga",
                    Username = "nael_dau",
                    RoleId = adminRole!.Id,
                    Role = adminRole,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                }
            );
        }
    }
}