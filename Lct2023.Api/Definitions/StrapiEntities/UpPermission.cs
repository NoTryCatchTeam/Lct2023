using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("up_permissions")]
[Index("CreatedById", Name = "up_permissions_created_by_id_fk")]
[Index("UpdatedById", Name = "up_permissions_updated_by_id_fk")]
public class UpPermission
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
    [InverseProperty("UpPermissionCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<UpPermissionsRoleLink> UpPermissionsRoleLinks { get; set; } = new List<UpPermissionsRoleLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("UpPermissionUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
