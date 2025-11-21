using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Keyless]
[Table("orarul")]
public partial class Orarul
{
    [Column("Id_Disciplina")]
    public int IdDisciplina { get; set; }

    [Column("Id_Profesor")]
    public int IdProfesor { get; set; }

    [Column("Id_Grupa")]
    public int IdGrupa { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Zi { get; set; } = null!;

    [Precision(0)]
    public TimeOnly Ora { get; set; }

    public int Auditoriu { get; set; }
}
