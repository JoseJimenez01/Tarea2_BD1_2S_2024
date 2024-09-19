using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tarea2_BD1.Models;

public partial class Dbtarea2Context : DbContext
{
    public Dbtarea2Context()
    {
    }

    public Dbtarea2Context(DbContextOptions<Dbtarea2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BitacoraEvento> BitacoraEventos { get; set; }

    public virtual DbSet<Dberror> Dberrors { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Error> Errors { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<TipoEvento> TipoEventos { get; set; }

    public virtual DbSet<TipoMovimiento> TipoMovimientos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DESKTOP-NVUTANU\\SQLEXPRESS; Initial Catalog=DBTarea2; user id= sa; pwd= 123.elSQLs.; Integrated Security=True; TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BitacoraEvento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bitacora__3213E83F43576EC7");

            entity.ToTable("BitacoraEvento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.IdPostByUser).HasColumnName("idPostByUser");
            entity.Property(e => e.IdTipoEvento).HasColumnName("idTipoEvento");
            entity.Property(e => e.PostInIp)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("PostInIP");
            entity.Property(e => e.PostTime).HasColumnType("datetime");

            entity.HasOne(d => d.IdPostByUserNavigation).WithMany(p => p.BitacoraEventos)
                .HasForeignKey(d => d.IdPostByUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BitacoraE__idPos__47DBAE45");

            entity.HasOne(d => d.IdTipoEventoNavigation).WithMany(p => p.BitacoraEventos)
                .HasForeignKey(d => d.IdTipoEvento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BitacoraE__idTip__46E78A0C");
        });

        modelBuilder.Entity<Dberror>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DBError");

            entity.Property(e => e.ErrorDateTime).HasColumnType("datetime");
            entity.Property(e => e.ErrorId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ErrorID");
            entity.Property(e => e.ErrorMessage).IsUnicode(false);
            entity.Property(e => e.ErrorProcedure).IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empleado__3213E83F53F89BBB");

            entity.ToTable("Empleado");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdPuesto).HasColumnName("idPuesto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.SaldoVacaciones).HasColumnType("money");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdPuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Empleado__idPues__398D8EEE");
        });

        modelBuilder.Entity<Error>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Error__3213E83F711974FE");

            entity.ToTable("Error");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(128)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movimien__3213E83FA926C626");

            entity.ToTable("Movimiento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");
            entity.Property(e => e.IdPostByUser).HasColumnName("idPostByUser");
            entity.Property(e => e.IdTipoMovimiento).HasColumnName("idTipoMovimiento");
            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.NuevoSaldo).HasColumnType("money");
            entity.Property(e => e.PostInIp)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("PostInIP");
            entity.Property(e => e.PostTime).HasColumnType("datetime");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__idEmp__403A8C7D");

            entity.HasOne(d => d.IdPostByUserNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdPostByUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__idPos__4222D4EF");

            entity.HasOne(d => d.IdTipoMovimientoNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdTipoMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__idTip__412EB0B6");
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Puesto__3213E83F36FD3EEF");

            entity.ToTable("Puesto");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.SalarioXhora)
                .HasColumnType("money")
                .HasColumnName("SalarioXHora");
        });

        modelBuilder.Entity<TipoEvento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoEven__3213E83F49563BB8");

            entity.ToTable("TipoEvento");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoMovimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoMovi__3213E83F94436E18");

            entity.ToTable("TipoMovimiento");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.TipoAccion)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3213E83F4278D7D5");

            entity.ToTable("Usuario");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
