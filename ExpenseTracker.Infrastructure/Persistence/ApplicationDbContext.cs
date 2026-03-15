using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence;

public sealed class ApplicationDbContext: DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserCredentials> UserCredentials => Set<UserCredentials>();
    public DbSet<Expense> Expenses => Set<Expense>();
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}