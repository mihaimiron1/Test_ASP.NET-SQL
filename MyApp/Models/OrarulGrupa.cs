using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

[Keyless]
[Table("orarul_grupa")]
public partial class OrarulGrupa
{
    [Column("Id_Disciplina")]
    public int? IdDisciplina { get; set; }

    [Column("Cod_Grupa")]
    [StringLength(6)]
    [Unicode(false)]
    public string? CodGrupa { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string? Zi { get; set; }

    public TimeOnly? Ora { get; set; }

    public int? Auditoriu { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Bloc { get; set; }

    [Column("Id_Orarul_Grupa")]
    public int IdOrarulGrupa { get; set; }

    [Column("Data_inserare", TypeName = "datetime")]
    public DateTime DataInserare { get; set; }
}
