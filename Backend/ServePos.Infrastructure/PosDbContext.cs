using Microsoft.EntityFrameworkCore;
using ServePos.Domain.Entities;
using ServePos.Shared;
using MenuItemKind = ServePos.Shared.MenuItemType;

namespace ServePos.Infrastructure
{
    public class PosDbContext(DbContextOptions<PosDbContext> options) : DbContext(options)
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CashPayment> CashPayments { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketItem> TicketItems { get; set; }
        public DbSet<OrderCancellation> OrderCancellations { get; set; }
        public DbSet<AuditEvent> AuditEvents { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<ServingStation> ServingStations { get; set; }
        public DbSet<Domain.Entities.MenuItemType> MenuItemTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresEnum<MenuItemKind>();

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
                .HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(x => x.CashPayment)
                .WithOne(x => x.Order)
                .HasForeignKey<CashPayment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CashPayment>()
                .HasOne(x => x.ReceivedBy)
                .WithMany()
                .HasForeignKey(x => x.ReceivedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasMany(x => x.Items)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketItem>()
                .HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderCancellation>()
                .HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderCancellation>()
                .HasOne(x => x.PerformedBy)
                .WithMany()
                .HasForeignKey(x => x.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditEvent>()
                .HasOne(x => x.Staff)
                .WithMany()
                .HasForeignKey(x => x.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasIndex(x => x.ClientOrderId)
                .IsUnique()
                .HasFilter("\"ClientOrderId\" IS NOT NULL");

            modelBuilder.Entity<Event>()
                .HasIndex(x => x.IsActive)
                .IsUnique()
                .HasFilter("\"IsActive\" = TRUE");

            modelBuilder.Entity<Role>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .HasIndex(x => new { x.EventId, x.Name })
                .IsUnique();

            modelBuilder.Entity<Table>()
                .HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServingStation>()
                .HasIndex(x => new { x.EventId, x.Name })
                .IsUnique();

            modelBuilder.Entity<ServingStation>()
                .HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Domain.Entities.MenuItemType>()
                .HasIndex(x => new { x.ServingStationId, x.Name })
                .IsUnique();

            modelBuilder.Entity<Domain.Entities.MenuItemType>()
                .HasOne(x => x.ServingStation)
                .WithMany()
                .HasForeignKey(x => x.ServingStationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}