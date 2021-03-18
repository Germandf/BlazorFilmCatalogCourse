using Microsoft.EntityFrameworkCore;
using GdFilms.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GdFilms.Server.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilmGenre>().HasKey(x => new { x.GenreId, x.FilmId });
            modelBuilder.Entity<FilmActor>().HasKey(x => new { x.PersonId, x.FilmId });
            var adminRole = new IdentityRole() { Id = "3e9f7823-6f61-429b-b3b1-21baed9bc159", Name = "admin", NormalizedName = "admin" };
            modelBuilder.Entity<IdentityRole>().HasData(adminRole);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<FilmActor> FilmActors { get; set; }
        public DbSet<FilmGenre> FilmGenres { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<FilmVote> FilmVotes { get; set; }
    }
}
