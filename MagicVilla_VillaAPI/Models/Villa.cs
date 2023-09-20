using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Npgsql;
using static NpgsqlTypes.NpgsqlDbType;

namespace MagicVilla_VillaAPI.Models;

public class Villa
{
    // di default, le prop sono tutte required, per disabilitare questa funzione cambiare in "disable" la propietà "<nullable> nel file .csproj
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Details { get; set; }
    public double Rate { get; set; }
    public int? Sqft { get; set; }
    public int? Occupancy { get; set; }
    public string? ImageUrl { get; set; }
    public string? Amenity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Villa()
    {
        this.CreatedDate = DateTime.UtcNow;
    }
}