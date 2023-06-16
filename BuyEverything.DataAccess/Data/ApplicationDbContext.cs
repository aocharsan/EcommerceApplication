using BuyEverything.Models;
using Microsoft.EntityFrameworkCore;

namespace BuyEverything.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
       
        
        
       
                
        }
        //create a columns in Categories table
        public DbSet<Category> Categories { get; set; }

        //VIMP -->when anything is to be updated to database ,we have to use add-migration <name> 


        //seed data to category table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Category>().HasData(
                new Category { Id=1 ,Name="Action",DisplayOrder=1},
                new Category { Id =2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id =3, Name = "History", DisplayOrder = 3 }

                );
        }





    }
}
