using B2B.Dao.Models;
using Microsoft.EntityFrameworkCore;

namespace B2B.Dao.Contexts
{
    public partial class B2BDbContext : DbContext
    {
        public B2BDbContext()
        {
        }

        public B2BDbContext(DbContextOptions<B2BDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Twb2bSysmenu> Twb2bSysmenus { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("REX")
                .UseCollation("USING_NLS_COMP");

            modelBuilder.Entity<Twb2bSysmenu>(entity =>
            {
                entity.ToTable("TWB2B_SYSMENU");

                entity.Property(e => e.Id)
                    .HasPrecision(3)
                    .HasColumnName("ID");

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ICON");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Prev)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PREV");

                entity.Property(e => e.Seq)
                    .HasPrecision(3)
                    .HasColumnName("SEQ");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
