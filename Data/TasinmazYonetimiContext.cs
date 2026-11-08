using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Data;

public partial class TasinmazYonetimiContext : DbContext
{
    public TasinmazYonetimiContext(DbContextOptions<TasinmazYonetimiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Durumlar> Durumlars { get; set; }

    public virtual DbSet<Ilceler> Ilcelers { get; set; }

    public virtual DbSet<Iller> Illers { get; set; }

    public virtual DbSet<IslemTipleri> IslemTipleris { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<Loglar> Loglars { get; set; }

    public virtual DbSet<Mahalleler> Mahallelers { get; set; }

    public virtual DbSet<Roller> Rollers { get; set; }

    public virtual DbSet<Tasinmazlar> Tasinmazlars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Durumlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Durumlar_pkey");

            entity.ToTable("Durumlar");

            entity.HasIndex(e => e.DurumAdi, "UQ_Durumlar_DurumAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.DurumAdi).HasMaxLength(30);
        });

        modelBuilder.Entity<Ilceler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ilceler_pkey");

            entity.ToTable("Ilceler");

            entity.HasIndex(e => new { e.IlId, e.IlceAdi }, "UQ_Ilceler_IlId_IlceAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.IlceAdi).HasMaxLength(100);

            entity.HasOne(d => d.Il).WithMany(p => p.Ilcelers)
                .HasForeignKey(d => d.IlId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ilceler_Iller");
        });

        modelBuilder.Entity<Iller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Iller_pkey");

            entity.ToTable("Iller");

            entity.HasIndex(e => e.IlAdi, "UQ_Iller_IlAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.IlAdi).HasMaxLength(100);
        });

        modelBuilder.Entity<IslemTipleri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("IslemTipleri_pkey");

            entity.ToTable("IslemTipleri");

            entity.HasIndex(e => e.IslemTipiAdi, "UQ_IslemTipleri_IslemTipiAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.IslemTipiAdi).HasMaxLength(50);
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Kullanicilar_pkey");

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Email, "UQ_Kullanicilar_Email").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Ad).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Soyad).HasMaxLength(50);

            entity.HasOne(d => d.Rol).WithMany(p => p.Kullanicilars)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kullanici_Rol");
        });

        modelBuilder.Entity<Loglar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Loglar_pkey");

            entity.ToTable("Loglar");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.KullaniciIp).HasColumnType("character varying");
            entity.Property(e => e.TarihSaat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Durum).WithMany(p => p.Loglars)
                .HasForeignKey(d => d.DurumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loglar_Durumlar");

            entity.HasOne(d => d.IslemTipi).WithMany(p => p.Loglars)
                .HasForeignKey(d => d.IslemTipiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loglar_IslemTipleri");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Loglars)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Loglar_Kullanicilar");
        });

        modelBuilder.Entity<Mahalleler>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Mahalleler_pkey");

            entity.ToTable("Mahalleler");

            entity.HasIndex(e => new { e.IlceId, e.MahalleAdi }, "UQ_Mahalleler_IlceId_MahalleAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.MahalleAdi).HasMaxLength(100);

            entity.HasOne(d => d.Ilce).WithMany(p => p.Mahallelers)
                .HasForeignKey(d => d.IlceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mahalleler_Ilceler");
        });

        modelBuilder.Entity<Roller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roller_pkey");

            entity.ToTable("Roller");

            entity.HasIndex(e => e.RolAdi, "UQ_Roller_RolAdi").IsUnique();

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.RolAdi).HasMaxLength(30);
        });

        modelBuilder.Entity<Tasinmazlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tasinmazlar_pkey");

            entity.ToTable("Tasinmazlar");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.Ada).HasMaxLength(30);
            entity.Property(e => e.Boylam).HasPrecision(9, 6);
            entity.Property(e => e.Enlem).HasPrecision(9, 6);
            entity.Property(e => e.KayitTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Nitelik).HasMaxLength(20);
            entity.Property(e => e.Parsel).HasMaxLength(30);

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Tasinmazlars)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasinmazlar_Kullanicilar");

            entity.HasOne(d => d.Mahalle).WithMany(p => p.Tasinmazlars)
                .HasForeignKey(d => d.MahalleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasinmazlar_Mahalleler");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
