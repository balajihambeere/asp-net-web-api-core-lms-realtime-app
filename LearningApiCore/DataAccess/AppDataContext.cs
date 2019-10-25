namespace LearningApiCore.DataAccess
{
    using LearningApiCore.DataAccess.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AppDataContext : IdentityDbContext<AppUser>
    {
        public DbSet<Category> Category { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Lesson> Lesson { get; set; }
        public DbSet<Topic> Topic { get; set; }
        public DbSet<Language> Language { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            LocalDbConnection(optionsBuilder);
           // AzureDbConnection(optionsBuilder);
        }

        private void AzureDbConnection(DbContextOptionsBuilder optionsBuilder)
        {
            var prodConn = "Server=tcp:example.database.windows.net,1433;Initial Catalog=lmsdb;Persist Security Info=False;User ID=raptei;Password=raptei@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            optionsBuilder.UseSqlServer(prodConn);
        }

        private void LocalDbConnection(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=DESKTOP-612CT87;Database=LearningDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
