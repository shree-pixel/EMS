
using EMS.Models;
using Microsoft.EntityFrameworkCore;
namespace EMS.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {


    }
    public DbSet<DetailsModel> Details { get; set; }
}

