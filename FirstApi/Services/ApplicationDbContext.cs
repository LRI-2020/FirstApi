using System.Data.Common;
using FirstApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FirstApi.Services;

public class ApplicationDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<BookTypeEntity> BookTypes { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        var intArrayValueConverter = new ValueConverter<List<int>, string>(
            i => string.Join(",", i),
            s => string.IsNullOrWhiteSpace(s) ? new List<int>() : s.Split(",", StringSplitOptions.None).Select(int.Parse).ToList());
        
        modelBuilder
            .Entity<Book>()
            .Property(e => e.Type)
            .HasConversion<int>();

        modelBuilder.Entity<Book>()
            .HasKey(b => b.Id);
        
        modelBuilder.Entity<Book>()
            .Property(b=>b.Ratings)
            .HasConversion(intArrayValueConverter);

        modelBuilder.Entity<BookTypeEntity>()
            .HasData(
                new BookTypeEntity { Id = 1, BookType = "Unknown" },
                new BookTypeEntity { Id = 2, BookType = "Fiction" },
                new BookTypeEntity { Id = 3, BookType = "Comic" },
                new BookTypeEntity { Id = 4, BookType = "Biography" },
                new BookTypeEntity { Id = 5, BookType = "Conte" });
    }
}