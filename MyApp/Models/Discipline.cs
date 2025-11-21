using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Table("discipline")]
public partial class Discipline
{
    [Key]
    [Column("Id_Disciplina")]
    public int IdDisciplina { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Disciplina { get; set; } = null!;

    [Column("Nr_ore_plan_disciplina")]
    public int NrOrePlanDisciplina { get; set; }

    [InverseProperty("IdDisciplinaNavigation")]
    public virtual ICollection<StudentiReusitum> StudentiReusita { get; set; } = new List<StudentiReusitum>();
}
