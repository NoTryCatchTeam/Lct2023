using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("up_permissions_role_links")]
[Index("PermissionId", Name = "up_permissions_role_links_fk")]
[Index("RoleId", Name = "up_permissions_role_links_inv_fk")]
[Index("PermissionOrder", Name = "up_permissions_role_links_order_inv_fk")]
[Index("PermissionId", "RoleId", Name = "up_permissions_role_links_unique", IsUnique = true)]
public class UpPermissionsRoleLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("permission_id")]
    public int? PermissionId { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("permission_order")]
    public double? PermissionOrder { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("UpPermissionsRoleLinks")]
    public virtual UpPermission? Permission { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UpPermissionsRoleLinks")]
    public virtual UpRole? Role { get; set; }
}
