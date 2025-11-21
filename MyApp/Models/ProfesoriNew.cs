using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Table("profesori_new")]
public partial class ProfesoriNew
{
    [Key]
    [Column("Id_Profesor")]
    public int IdProfesor { get; set; }

    [Column("Nume_Profesor")]
    [StringLength(50)]
    [Unicode(false)]
    public string NumeProfesor { get; set; } = null!;

    [Column("Prenume_Profesor")]
    [StringLength(50)]
    [Unicode(false)]
    public string PrenumeProfesor { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Localitate { get; set; }

    [Column("Adresa_1")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Adresa1 { get; set; }

    [Column("Adresa_2")]
    [StringLength(200)]
    [Unicode(false)]
    public string? Adresa2 { get; set; }
}
