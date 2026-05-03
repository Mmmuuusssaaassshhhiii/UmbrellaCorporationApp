using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var connectionString = "server=localhost;port=8889;database=UmbrellaDb;user=root;password=root;";

            var options = new DbContextOptionsBuilder<UmbrellaDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            var context = new UmbrellaDbContext(options);

            ApplicationConfiguration.Initialize(); 
            Application.Run(new AuthForm(context)); 
        }
    }
}