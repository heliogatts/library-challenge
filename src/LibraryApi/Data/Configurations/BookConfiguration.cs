using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryApi.Domain;
using LibraryApi.Domain.ValueObjects;

namespace LibraryApi.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(13)
            .HasConversion(
                isbn => isbn.Value,
                value => Isbn.Create(value));

        builder.HasIndex(b => b.ISBN).IsUnique();

        builder.Property(b => b.PublishedYear).IsRequired();

        builder.Property(b => b.Description).HasMaxLength(2000);

        builder.Property(b => b.CreatedAt).HasDefaultValueSql("now()");

        // RESTRICT — no cascade deletes
        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
