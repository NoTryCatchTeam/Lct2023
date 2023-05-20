using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("up_users_role_links")]
[Index("UserId", Name = "up_users_role_links_fk")]
[Index("RoleId", Name = "up_users_role_links_inv_fk")]
[Index("UserOrder", Name = "up_users_role_links_order_inv_fk")]
[Index("UserId", "RoleId", Name = "up_users_role_links_unique", IsUnique = true)]
public class UpUsersRoleLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("user_order")]
    public double? UserOrder { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UpUsersRoleLinks")]
    public virtual UpRole? Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UpUsersRoleLinks")]
    public virtual UpUser? User { get; set; }
}
