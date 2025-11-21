using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[PrimaryKey("IdStudent", "IdDisciplina", "IdProfesor", "IdGrupa", "TipEvaluare")]
[Table("studenti_reusita")]
public partial class StudentiReusitum
{
    [Key]
    [Column("Id_Student")]
    public int IdStudent { get; set; }

    [Key]
    [Column("Id_Disciplina")]
    public int IdDisciplina { get; set; }

    [Key]
    [Column("Id_Profesor")]
    public int IdProfesor { get; set; }

    [Key]
    [Column("Id_Grupa")]
    public int IdGrupa { get; set; }

    [Key]
    [Column("Tip_Evaluare")]
    [StringLength(50)]
    [Unicode(false)]
    public string TipEvaluare { get; set; } = null!;

    public int? Nota { get; set; }

    [Column("Data_Evaluare")]
    public DateOnly? DataEvaluare { get; set; }

    [ForeignKey("IdDisciplina")]
    [InverseProperty("StudentiReusita")]
    public virtual Discipline IdDisciplinaNavigation { get; set; } = null!;

    [ForeignKey("IdGrupa")]
    [InverseProperty("StudentiReusita")]
    public virtual Grupe IdGrupaNavigation { get; set; } = null!;

    [ForeignKey("IdProfesor")]
    [InverseProperty("StudentiReusita")]
    public virtual Profesori IdProfesorNavigation { get; set; } = null!;

    [ForeignKey("IdStudent")]
    [InverseProperty("StudentiReusita")]
    public virtual Studenti IdStudentNavigation { get; set; } = null!;
}
