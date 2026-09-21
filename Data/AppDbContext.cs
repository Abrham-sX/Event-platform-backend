using Microsoft.EntityFrameworkCore;
using EP.API.Entities;

namespace EP.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<EventSpeaker> EventSpeakers => Set<EventSpeaker>();
    public DbSet<EventTimeline> EventTimelines => Set<EventTimeline>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Event ---
        modelBuilder.Entity<Event>(e =>
        {
            e.ToTable("Events");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(300);
            e.Property(x => x.Description).HasMaxLength(4000);
            e.Property(x => x.ShortDescription).HasMaxLength(500);
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
            e.Property(x => x.Currency).HasMaxLength(3);
            e.Property(x => x.ImageUrl).HasMaxLength(1000);
            e.Property(x => x.Objectives).HasMaxLength(2000);
            e.Property(x => x.RiskManagementPlan).HasMaxLength(4000);
            e.Property(x => x.PermitsNotes).HasMaxLength(2000);
            e.Property(x => x.TransportationNotes).HasMaxLength(2000);
            e.Property(x => x.EquipmentNotes).HasMaxLength(2000);
            e.Property(x => x.CateringNotes).HasMaxLength(2000);
            e.Property(x => x.TicketPrice).HasPrecision(18, 2);
            e.Property(x => x.Budget).HasPrecision(18, 2);
            e.HasIndex(x => x.StartDate);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.IsActive);

            e.HasOne(x => x.Category)
             .WithMany(c => c.Events)
             .HasForeignKey(x => x.EventCategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Venue)
             .WithMany(v => v.Events)
             .HasForeignKey(x => x.VenueId)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasMany(x => x.Timelines)
             .WithOne(t => t.Event)
             .HasForeignKey(t => t.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // --- EventCategory ---
        modelBuilder.Entity<EventCategory>(c =>
        {
            c.ToTable("EventCategories");
            c.HasKey(x => x.Id);
            c.Property(x => x.Name).IsRequired().HasMaxLength(150);
            c.Property(x => x.Description).HasMaxLength(500);
            c.Property(x => x.IconUrl).HasMaxLength(500);
            c.HasIndex(x => x.Name);
            c.HasIndex(x => x.IsActive);
        });

        // --- Venue ---
        modelBuilder.Entity<Venue>(v =>
        {
            v.ToTable("Venues");
            v.HasKey(x => x.Id);
            v.Property(x => x.Name).IsRequired().HasMaxLength(250);
            v.Property(x => x.Description).HasMaxLength(1000);
            v.Property(x => x.Address).IsRequired().HasMaxLength(500);
            v.Property(x => x.City).HasMaxLength(150);
            v.Property(x => x.State).HasMaxLength(150);
            v.Property(x => x.ZipCode).HasMaxLength(20);
            v.Property(x => x.Country).HasMaxLength(100);
            v.Property(x => x.ContactPhone).HasMaxLength(50);
            v.Property(x => x.ContactEmail).HasMaxLength(200);
            v.Property(x => x.Amenities).HasMaxLength(2000);
            v.HasIndex(x => x.Name);
            v.HasIndex(x => x.IsActive);
        });

        // --- Speaker ---
        modelBuilder.Entity<Speaker>(s =>
        {
            s.ToTable("Speakers");
            s.HasKey(x => x.Id);
            s.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            s.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            s.Property(x => x.Bio).HasMaxLength(2000);
            s.Property(x => x.PhotoUrl).HasMaxLength(1000);
            s.Property(x => x.Email).HasMaxLength(200);
            s.Property(x => x.Phone).HasMaxLength(50);
            s.Property(x => x.Company).HasMaxLength(200);
            s.Property(x => x.Title).HasMaxLength(200);
            s.Property(x => x.Website).HasMaxLength(500);
            s.HasIndex(x => x.Email);
            s.HasIndex(x => x.LastName);
            s.HasIndex(x => x.IsActive);
        });

        // --- EventSpeaker (join) ---
        modelBuilder.Entity<EventSpeaker>(es =>
        {
            es.ToTable("EventSpeakers");
            es.HasKey(x => x.Id);
            es.Property(x => x.Role).HasMaxLength(100);
            es.HasIndex(x => new { x.EventId, x.SpeakerId }).IsUnique();

            es.HasOne(x => x.Event)
              .WithMany(e => e.EventSpeakers)
              .HasForeignKey(x => x.EventId)
              .OnDelete(DeleteBehavior.Cascade);

            es.HasOne(x => x.Speaker)
              .WithMany(s => s.EventSpeakers)
              .HasForeignKey(x => x.SpeakerId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // --- EventTimeline ---
        modelBuilder.Entity<EventTimeline>(tl =>
        {
            tl.ToTable("EventTimelines");
            tl.HasKey(x => x.Id);
            tl.Property(x => x.Title).IsRequired().HasMaxLength(300);
            tl.Property(x => x.Description).HasMaxLength(1000);
            tl.Property(x => x.Status).IsRequired().HasMaxLength(50);
            tl.Property(x => x.AssignedTo).HasMaxLength(200);
            tl.HasIndex(x => new { x.EventId, x.SortOrder });
            tl.HasIndex(x => x.IsActive);

            tl.HasOne(x => x.Event)
              .WithMany(e => e.Timelines)
              .HasForeignKey(x => x.EventId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedCategories(modelBuilder);
        SeedVenues(modelBuilder);
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventCategory>().HasData(
            new EventCategory { Id = 1, Name = "Expo", Description = "Trade expositions and exhibitions" },
            new EventCategory { Id = 2, Name = "Religious", Description = "Religious gatherings and ceremonies" },
            new EventCategory { Id = 3, Name = "Meeting", Description = "Business and organizational meetings" },
            new EventCategory { Id = 4, Name = "Convention", Description = "Large conventions and conferences" },
            new EventCategory { Id = 5, Name = "Tradeshow", Description = "Industry trade shows" },
            new EventCategory { Id = 6, Name = "Fundraiser", Description = "Fundraising events" },
            new EventCategory { Id = 7, Name = "Team Building", Description = "Team building activities" },
            new EventCategory { Id = 8, Name = "Wedding", Description = "Wedding ceremonies and receptions" },
            new EventCategory { Id = 9, Name = "Anniversary", Description = "Anniversary celebrations" },
            new EventCategory { Id = 10, Name = "Birthday", Description = "Birthday parties and celebrations" },
            new EventCategory { Id = 11, Name = "Concert", Description = "Music concerts and performances" },
            new EventCategory { Id = 12, Name = "Workshop", Description = "Educational workshops and seminars" },
            new EventCategory { Id = 13, Name = "Festival", Description = "Cultural and community festivals" },
            new EventCategory { Id = 14, Name = "Networking", Description = "Professional networking events" },
            new EventCategory { Id = 15, Name = "Sports", Description = "Sports events and tournaments" }
        );
    }

    private static void SeedVenues(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>().HasData(
            new Venue { Id = 1, Name = "Grand Convention Center", Address = "100 Main Street", City = "Nairobi", State = "Nairobi", Country = "Kenya", Capacity = 5000, ContactEmail = "info@grandcc.co.ke" },
            new Venue { Id = 2, Name = "Sunset Gardens", Address = "45 Riverside Drive", City = "Nairobi", State = "Nairobi", Country = "Kenya", Capacity = 500, ContactEmail = "info@sunsetgardens.co.ke" },
            new Venue { Id = 3, Name = "The Rooftop Lounge", Address = "88 Kenyatta Ave", City = "Nairobi", State = "Nairobi", Country = "Kenya", Capacity = 150, ContactEmail = "hello@rooftoplounge.co.ke" }
        );
    }
}
