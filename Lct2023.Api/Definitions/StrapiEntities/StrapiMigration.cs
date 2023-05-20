using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_migrations")]
public class StrapiMigration
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("time", TypeName = "timestamp without time zone")]
    public DateTime? Time { get; set; }
}
