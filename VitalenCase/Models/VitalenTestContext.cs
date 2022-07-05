using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VitalenCase.Models
{
    public partial class VitalenTestContext : DbContext
    {
        public VitalenTestContext()
        {
        }

        public VitalenTestContext(DbContextOptions<VitalenTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Logging> Loggings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("DATA SOURCE=VitalenTest.mssql.somee.com;Database=VitalenTest;PERSIST SECURITY INFO=True;USER ID=bunyamin_SQLLogin_1;PASSWORD=qgc9r3rhiz");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logging>(entity =>
            {
                entity.ToTable("Logging");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Method).HasMaxLength(10);

                entity.Property(e => e.RequestTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
