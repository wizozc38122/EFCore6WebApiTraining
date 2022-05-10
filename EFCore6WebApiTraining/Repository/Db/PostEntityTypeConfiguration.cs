using Microsoft.EntityFrameworkCore;
using EFCore6WebApiTraining.Repository.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore6WebApiTraining.Repository.Db
{
    public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Post");
            builder.HasKey(post => post.PostId);
            builder.Property(post => post.BlogId).HasColumnName("BlogId").IsRequired();
            builder.Property(post => post.Title).HasMaxLength(50).HasColumnName("Title").IsRequired();
            builder.Property(post => post.Content).HasMaxLength(250).HasColumnName("Content").IsRequired();
        }
    }
}
