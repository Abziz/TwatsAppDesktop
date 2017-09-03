using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TwatsAppCore.Models
{
    public class TwatsAppContext : DbContext
    {
        public TwatsAppContext() : base()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TwatsAppContext>());
            Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<TwatsAppUser> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
