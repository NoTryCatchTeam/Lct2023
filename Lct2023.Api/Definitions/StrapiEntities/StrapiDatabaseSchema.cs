using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_database_schema")]
public class StrapiDatabaseSchema
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("schema", TypeName = "json")]
    public string? Schema { get; set; }

    [Column("time", TypeName = "timestamp without time zone")]
    public DateTime? Time { get; set; }

    [Column("hash")]
    [StringLength(255)]
    public string? Hash { get; set; }
}
