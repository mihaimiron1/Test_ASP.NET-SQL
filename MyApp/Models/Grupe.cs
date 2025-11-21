using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Table("grupe")]
public partial class Grupe
{
    [Key]
    [Column("Id_Grupa")]
    public int IdGrupa { get; set; }

    [Column("Cod_Grupa")]
    [StringLength(20)]
    [Unicode(false)]
    public string CodGrupa { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Specialitate { get; set; } = null!;

    [Column("Nume_Facultate")]
    [StringLength(100)]
    [Unicode(false)]
    public string NumeFacultate { get; set; } = null!;

    [InverseProperty("IdGrupaNavigation")]
    public virtual ICollection<StudentiReusitum> StudentiReusita { get; set; } = new List<StudentiReusitum>();
}
