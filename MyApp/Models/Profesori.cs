using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Table("profesori")]
public partial class Profesori
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

    [Column("Adresa_Postala_Profesor")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AdresaPostalaProfesor { get; set; }

    [InverseProperty("IdProfesorNavigation")]
    public virtual ICollection<StudentiReusitum> StudentiReusita { get; set; } = new List<StudentiReusitum>();
}
