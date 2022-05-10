using Microsoft.EntityFrameworkCore;
using EFCore6WebApiTraining.Repository.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore6WebApiTraining.Repository.Db
{
    public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blog");
            builder.HasKey(blog => blog.BlogId);
            builder.Property(blog => blog.Url).HasMaxLength(250).HasColumnName("Url").IsRequired();

            builder.HasMany(blog => blog.Posts)
                .WithOne(post => post.Blog)
                .HasForeignKey(post => post.BlogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
