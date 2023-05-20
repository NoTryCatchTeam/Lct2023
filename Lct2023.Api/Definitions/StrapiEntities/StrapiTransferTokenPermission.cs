using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_transfer_token_permissions")]
[Index("CreatedById", Name = "strapi_transfer_token_permissions_created_by_id_fk")]
[Index("UpdatedById", Name = "strapi_transfer_token_permissions_updated_by_id_fk")]
public class StrapiTransferTokenPermission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("action")]
    [StringLength(255)]
    public string? Action { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("StrapiTransferTokenPermissionCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("TransferTokenPermission")]
    public virtual ICollection<StrapiTransferTokenPermissionsTokenLink> StrapiTransferTokenPermissionsTokenLinks { get; set; } = new List<StrapiTransferTokenPermissionsTokenLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("StrapiTransferTokenPermissionUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
