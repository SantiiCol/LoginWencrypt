using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LOGIN.Models;

public partial class BdLoginContext : DbContext
{
    public BdLoginContext()
    {
    }

    public BdLoginContext(DbContextOptions<BdLoginContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__5B65BF97533689E1");

            entity.ToTable("USUARIO");

            entity.Property(e => e.Clave)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
