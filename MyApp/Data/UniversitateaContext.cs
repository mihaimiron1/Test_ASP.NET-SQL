using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Data;

public partial class UniversitateaContext : DbContext
{
    public UniversitateaContext()
    {
    }

    public UniversitateaContext(DbContextOptions<UniversitateaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Grupe> Grupes { get; set; }

    public virtual DbSet<Orarul> Oraruls { get; set; }

    public virtual DbSet<OrarulGrupa> OrarulGrupas { get; set; }

    public virtual DbSet<Profesori> Profesoris { get; set; }

    public virtual DbSet<ProfesoriNew> ProfesoriNews { get; set; }

    public virtual DbSet<Studenti> Studentis { get; set; }

    public virtual DbSet<StudentiReusitum> StudentiReusita { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MIHAI;Initial Catalog=universitatea;Persist Security Info=True;User ID=sa;Password=eusuntmisa004;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.IdDisciplina).HasName("pk_discipline");

            entity.Property(e => e.IdDisciplina).ValueGeneratedNever();
        });

        modelBuilder.Entity<Grupe>(entity =>
        {
            entity.HasKey(e => e.IdGrupa).HasName("pk_grupe");

            entity.Property(e => e.IdGrupa).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrarulGrupa>(entity =>
        {
            entity.Property(e => e.Bloc)
                .HasDefaultValue("B")
                .IsFixedLength();
            entity.Property(e => e.CodGrupa).IsFixedLength();
            entity.Property(e => e.DataInserare).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdOrarulGrupa).ValueGeneratedOnAdd();
            entity.Property(e => e.Zi).IsFixedLength();
        });

        modelBuilder.Entity<Profesori>(entity =>
        {
            entity.HasKey(e => e.IdProfesor).HasName("pk_profesori");

            entity.Property(e => e.IdProfesor).ValueGeneratedNever();
        });

        modelBuilder.Entity<ProfesoriNew>(entity =>
        {
            entity.HasKey(e => e.IdProfesor).HasName("PK__profesor__45D4152A41B6EA30");

            entity.Property(e => e.IdProfesor).ValueGeneratedNever();
            entity.Property(e => e.Localitate).HasDefaultValue("mun.Chisinau");
        });

        modelBuilder.Entity<Studenti>(entity =>
        {
            entity.HasKey(e => e.IdStudent).HasName("pk_studenti");

            entity.Property(e => e.IdStudent).ValueGeneratedNever();
        });

        modelBuilder.Entity<StudentiReusitum>(entity =>
        {
            entity.HasKey(e => new { e.IdStudent, e.IdDisciplina, e.IdProfesor, e.IdGrupa, e.TipEvaluare }).HasName("pk_studenti_reusita");

            entity.HasOne(d => d.IdDisciplinaNavigation).WithMany(p => p.StudentiReusita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reusita_disciplina");

            entity.HasOne(d => d.IdGrupaNavigation).WithMany(p => p.StudentiReusita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reusita_grupa");

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.StudentiReusita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reusita_profesor");

            entity.HasOne(d => d.IdStudentNavigation).WithMany(p => p.StudentiReusita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reusita_student");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
