using Chatry.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Chatry.Data
{
    public class ChatryDbContext : DbContext
    {

        public ChatryDbContext(DbContextOptions<ChatryDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

    }
}
