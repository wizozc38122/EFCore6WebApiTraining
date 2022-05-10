using Microsoft.EntityFrameworkCore;
using EFCore6WebApiTraining.Repository.Entities;

namespace EFCore6WebApiTraining.Repository.Db
{
    
    public class Blogging2Context : DbContext
    {
        public DbSet<Blog>? Blogs { get; set; }
        public DbSet<Post>? Posts { get; set; }

        public Blogging2Context(DbContextOptions<BloggingContext> options) : base(options) 
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlogEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }

    
}
