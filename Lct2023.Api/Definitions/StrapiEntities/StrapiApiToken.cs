using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_api_tokens")]
[Index("CreatedById", Name = "strapi_api_tokens_created_by_id_fk")]
[Index("UpdatedById", Name = "strapi_api_tokens_updated_by_id_fk")]
public class StrapiApiToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("type")]
    [StringLength(255)]
    public string? Type { get; set; }

    [Column("access_key")]
    [StringLength(255)]
    public string? AccessKey { get; set; }

    [Column("last_used_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? LastUsedAt { get; set; }

    [Column("expires_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? ExpiresAt { get; set; }

    [Column("lifespan")]
    public long? Lifespan { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("StrapiApiTokenCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("ApiToken")]
    public virtual ICollection<StrapiApiTokenPermissionsTokenLink> StrapiApiTokenPermissionsTokenLinks { get; set; } = new List<StrapiApiTokenPermissionsTokenLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("StrapiApiTokenUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
