using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_core_store_settings")]
public class StrapiCoreStoreSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("key")]
    [StringLength(255)]
    public string? Key { get; set; }

    [Column("value")]
    public string? Value { get; set; }

    [Column("type")]
    [StringLength(255)]
    public string? Type { get; set; }

    [Column("environment")]
    [StringLength(255)]
    public string? Environment { get; set; }

    [Column("tag")]
    [StringLength(255)]
    public string? Tag { get; set; }
}
