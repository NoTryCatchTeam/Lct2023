using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_transfer_tokens")]
[Index("CreatedById", Name = "strapi_transfer_tokens_created_by_id_fk")]
[Index("UpdatedById", Name = "strapi_transfer_tokens_updated_by_id_fk")]
public class StrapiTransferToken
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
    [InverseProperty("StrapiTransferTokenCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("TransferToken")]
    public virtual ICollection<StrapiTransferTokenPermissionsTokenLink> StrapiTransferTokenPermissionsTokenLinks { get; set; } = new List<StrapiTransferTokenPermissionsTokenLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("StrapiTransferTokenUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
