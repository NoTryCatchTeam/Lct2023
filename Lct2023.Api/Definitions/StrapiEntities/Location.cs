using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("locations")]
[Index("CreatedById", Name = "locations_created_by_id_fk")]
[Index("UpdatedById", Name = "locations_updated_by_id_fk")]
public class Location
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("lat")]
    public double? Lat { get; set; }

    [Column("lon")]
    public double? Lon { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("published_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? PublishedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("LocationCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [ForeignKey("UpdatedById")]
    [InverseProperty("LocationUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
