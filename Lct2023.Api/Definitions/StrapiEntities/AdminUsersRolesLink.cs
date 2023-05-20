using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("admin_users_roles_links")]
[Index("UserId", Name = "admin_users_roles_links_fk")]
[Index("RoleId", Name = "admin_users_roles_links_inv_fk")]
[Index("RoleOrder", Name = "admin_users_roles_links_order_fk")]
[Index("UserOrder", Name = "admin_users_roles_links_order_inv_fk")]
[Index("UserId", "RoleId", Name = "admin_users_roles_links_unique", IsUnique = true)]
public class AdminUsersRolesLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("role_order")]
    public double? RoleOrder { get; set; }

    [Column("user_order")]
    public double? UserOrder { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AdminUsersRolesLinks")]
    public virtual AdminRole? Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AdminUsersRolesLinks")]
    public virtual AdminUser? User { get; set; }
}
