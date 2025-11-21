using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Table("studenti")]
public partial class Studenti
{
    [Key]
    [Column("Id_Student")]
    public int IdStudent { get; set; }

    [Column("Nume_Student")]
    [StringLength(50)]
    [Unicode(false)]
    public string NumeStudent { get; set; } = null!;

    [Column("Prenume_Student")]
    [StringLength(50)]
    [Unicode(false)]
    public string PrenumeStudent { get; set; } = null!;

    [Column("Data_Nastere_Student")]
    public DateOnly DataNastereStudent { get; set; }

    [Column("Adresa_Postala_Student")]
    [StringLength(200)]
    [Unicode(false)]
    public string? AdresaPostalaStudent { get; set; }

    [InverseProperty("IdStudentNavigation")]
    public virtual ICollection<StudentiReusitum> StudentiReusita { get; set; } = new List<StudentiReusitum>();
}
