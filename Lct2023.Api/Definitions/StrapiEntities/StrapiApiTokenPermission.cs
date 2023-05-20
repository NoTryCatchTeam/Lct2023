using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_api_token_permissions")]
[Index("CreatedById", Name = "strapi_api_token_permissions_created_by_id_fk")]
[Index("UpdatedById", Name = "strapi_api_token_permissions_updated_by_id_fk")]
public class StrapiApiTokenPermission
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
    [InverseProperty("StrapiApiTokenPermissionCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("ApiTokenPermission")]
    public virtual ICollection<StrapiApiTokenPermissionsTokenLink> StrapiApiTokenPermissionsTokenLinks { get; set; } = new List<StrapiApiTokenPermissionsTokenLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("StrapiApiTokenPermissionUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
