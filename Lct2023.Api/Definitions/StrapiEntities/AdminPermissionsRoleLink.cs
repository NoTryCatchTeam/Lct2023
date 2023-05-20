using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("admin_permissions_role_links")]
[Index("PermissionId", Name = "admin_permissions_role_links_fk")]
[Index("RoleId", Name = "admin_permissions_role_links_inv_fk")]
[Index("PermissionOrder", Name = "admin_permissions_role_links_order_inv_fk")]
[Index("PermissionId", "RoleId", Name = "admin_permissions_role_links_unique", IsUnique = true)]
public class AdminPermissionsRoleLink
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
    [InverseProperty("AdminPermissionsRoleLinks")]
    public virtual AdminPermission? Permission { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AdminPermissionsRoleLinks")]
    public virtual AdminRole? Role { get; set; }
}
