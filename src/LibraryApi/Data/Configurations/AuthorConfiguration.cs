using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryApi.Domain;

namespace LibraryApi.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(a => a.Name).IsUnique();

        builder.Property(a => a.CreatedAt).HasDefaultValueSql("now()");
    }
}
