using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace LawyerHelper.Models
{
    public class LawyerHelperContext : DbContext
    {
        public LawyerHelperContext(DbContextOptions<LawyerHelperContext> options)
            : base(options)
        {
        }

        public DbSet<Process> Process { get; set; } = null!;
        public DbSet<Process_Relation> Process_Relation { get; set; } = null!;
        public DbSet<History> History { get; set; } = null!;
        public DbSet<Customers> Customers { get; set; } = null!;
        public DbSet<Info> Info { get; set; } = null!;
        public DbSet<Lawyers> Lawyers { get; set; } = null!;
    }
}