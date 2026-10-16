using Microsoft.EntityFrameworkCore;
using Raceday.Api.Models;

namespace Raceday.Api.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<EventRoute> EventRoutes => Set<EventRoute>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // USERS
        b.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Role).HasDefaultValue("Participant");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            e.ToTable(t => t.HasCheckConstraint("CK_Users_Role",
                "[Role] IN ('Organiser','Participant')"));
        });

        // EVENTS
        b.Entity<Event>(e =>
        {
            e.ToTable("Events");
            e.HasKey(x => x.EventId);
            e.HasOne(x => x.Organiser)
             .WithMany(u => u.OrganisedEvents)
             .HasForeignKey(x => x.OrganiserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Events_Distance", "[DistanceKm] > 0");
                t.HasCheckConstraint("CK_Events_EventType", "[EventType] IN ('Run','Walk','Cycle')");
            });
        });

        // CATEGORIES
        b.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.HasKey(x => x.CategoryId);
            e.HasOne(x => x.Event)
             .WithMany(ev => ev.Categories)
             .HasForeignKey(x => x.EventId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.EventId, x.CategoryName }).IsUnique();
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Categories_MinAge", "[MinAge] IS NULL OR [MinAge] >= 0");
                t.HasCheckConstraint("CK_Categories_MaxAge", "[MaxAge] IS NULL OR [MaxAge] >= 0");
                t.HasCheckConstraint("CK_Categories_AgeRange", "[MaxAge] IS NULL OR [MinAge] IS NULL OR [MaxAge] >= [MinAge]");
                t.HasCheckConstraint("CK_Categories_Distance", "[CategoryDistanceKm] IS NULL OR [CategoryDistanceKm] > 0");
            });
        });

        // ENROLMENTS
        b.Entity<Enrolment>(e =>
        {
            e.ToTable("Enrolments");
            e.HasKey(x => x.EnrolmentId);
            e.HasOne(x => x.Participant)
             .WithMany(u => u.Enrolments)
             .HasForeignKey(x => x.ParticipantId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Event)
             .WithMany(ev => ev.Enrolments)
             .HasForeignKey(x => x.EventId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Category)
             .WithMany(c => c.Enrolments)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.ParticipantId, x.EventId }).IsUnique();
            e.Property(x => x.EnrolmentDate).HasDefaultValueSql("SYSDATETIME()");
            e.Property(x => x.Status).HasDefaultValue("Confirmed");
            e.ToTable(t => t.HasCheckConstraint("CK_Enrolments_Status",
                "[Status] IN ('Pending','Confirmed','Cancelled')"));
        });

        // RESULTS
        b.Entity<Result>(e =>
        {
            e.ToTable("Results");
            e.HasKey(x => x.ResultId);
            e.HasOne(x => x.Enrolment)
             .WithOne(en => en.Result)
             .HasForeignKey<Result>(x => x.EnrolmentId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.EnrolmentId).IsUnique();
            e.ToTable(t => t.HasCheckConstraint("CK_Results_Position", "[FinishingPosition] > 0"));
        });

        // EVENT ROUTES
        b.Entity<EventRoute>(e =>
        {
            e.ToTable("EventRoutes");
            e.HasKey(x => x.RouteId);
            e.HasOne(x => x.Event)
             .WithOne(ev => ev.Route)
             .HasForeignKey<EventRoute>(x => x.EventId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.EventId).IsUnique();
        });
    }
}
