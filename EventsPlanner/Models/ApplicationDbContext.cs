using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace EventsPlanner.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Event> EventsSubscribed { get; set; }
        public virtual ICollection<Event> EventsCreated { get; set; }

    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventField> EventFields { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<EventField>()
                .HasRequired<Event>(s => s.CurrentEvent)
                .WithMany(g => g.EventFields)
                .HasForeignKey<int>(s => s.eventId);


            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Event>(s => s.EventsCreated)
                .WithMany(c => c.ManageUsers)
                .Map(cs =>
                {
                    cs.MapLeftKey("userId");
                    cs.MapRightKey("eventId");
                    cs.ToTable("UserEventCreated");
                });


            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Event>(s => s.EventsSubscribed)
                .WithMany(c => c.SubscribedUsers)
                .Map(cs =>
                {
                    cs.MapLeftKey("userId");
                    cs.MapRightKey("eventId");
                    cs.ToTable("UserEventSubscribed");
                });


        }
        public ApplicationDbContext()
            : base("dbEventsPlanner")
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new IdentityDbInit());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(ApplicationDbContext context)
        {
            // настройки конфигурации контекста будут указываться здесь
        }
    }

    public class AppUserManager : UserManager<ApplicationUser>
    {
        public AppUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        { }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options,
            IOwinContext context)
        {
            ApplicationDbContext db = context.Get<ApplicationDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<ApplicationUser>(db));
            return manager;
        }
    }

}