using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UmbrellaCorp.Data
{
    public class UmbrellaDbContextFactory : IDesignTimeDbContextFactory<UmbrellaDbContext>
    {
        public UmbrellaDbContext CreateDbContext(string[] args)
        {
            var connectionString = "server=localhost;port=8889;database=UmbrellaDb;user=root;password=root;";

            var optionsBuilder = new DbContextOptionsBuilder<UmbrellaDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new UmbrellaDbContext(optionsBuilder.Options);
        }
    }
}