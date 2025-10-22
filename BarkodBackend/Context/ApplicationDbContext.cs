using BarkodBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BarkodBackend.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<TbFirma> Products { get; set; }
    }
}
