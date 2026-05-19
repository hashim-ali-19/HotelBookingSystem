using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Hotel> Hotels => Set<Hotel>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(booking => booking.BookingCode)
            .IsUnique();

        modelBuilder.Entity<Hotel>()
            .HasMany(hotel => hotel.Rooms)
            .WithOne(room => room.Hotel)
            .HasForeignKey(room => room.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Hotel>()
            .HasMany(hotel => hotel.Reviews)
            .WithOne(review => review.Hotel)
            .HasForeignKey(review => review.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Bookings)
            .WithOne(booking => booking.User)
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Reviews)
            .WithOne(review => review.User)
            .HasForeignKey(review => review.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Room>()
            .HasMany(room => room.Bookings)
            .WithOne(booking => booking.Room)
            .HasForeignKey(booking => booking.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasMany(booking => booking.Payments)
            .WithOne(payment => payment.Booking)
            .HasForeignKey(payment => payment.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var createdAt = entityType.FindProperty(nameof(BaseEntity.CreatedAt));
            if (createdAt is not null)
            {
                createdAt.SetDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }
}
