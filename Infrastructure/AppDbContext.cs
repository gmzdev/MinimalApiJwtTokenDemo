using Microsoft.EntityFrameworkCore;

public class AppDbContext: DbContext{
    public AppDbContext(DbContextOptions options)
        :base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
}