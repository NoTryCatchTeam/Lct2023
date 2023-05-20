using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("admin_permissions")]
[Index("CreatedById", Name = "admin_permissions_created_by_id_fk")]
[Index("UpdatedById", Name = "admin_permissions_updated_by_id_fk")]
public class AdminPermission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("action")]
    [StringLength(255)]
    public string? Action { get; set; }

    [Column("subject")]
    [StringLength(255)]
    public string? Subject { get; set; }

    [Column("properties", TypeName = "jsonb")]
    public string? Properties { get; set; }

    [Column("conditions", TypeName = "jsonb")]
    public string? Conditions { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<AdminPermissionsRoleLink> AdminPermissionsRoleLinks { get; set; } = new List<AdminPermissionsRoleLink>();

    [ForeignKey("CreatedById")]
    [InverseProperty("AdminPermissionCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [ForeignKey("UpdatedById")]
    [InverseProperty("AdminPermissionUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
