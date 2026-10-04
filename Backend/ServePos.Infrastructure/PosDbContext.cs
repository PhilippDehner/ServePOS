using Microsoft.EntityFrameworkCore;
using ServePos.Domain.Entities;
using ServePos.Shared;

namespace ServePos.Infrastructure
{
    public class PosDbContext(DbContextOptions<PosDbContext> options) : DbContext(options)
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<MenuItemType>();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.MenuItem)
                .WithMany()
                .HasForeignKey(o => o.MenuItemId);

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Type)
                .HasColumnType("menu_item_type");

            modelBuilder.Entity<Order>()
                .HasOne(x => x.EnteredBy)
                .WithMany()
                .HasForeignKey(x => x.EnteredById);

            modelBuilder.Entity<Order>()
                .HasIndex(x => x.ClientOrderId)
                .IsUnique()
                .HasFilter("\"ClientOrderId\" IS NOT NULL");
        }
    }
}