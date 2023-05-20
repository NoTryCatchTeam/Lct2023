using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("admin_roles")]
[Index("CreatedById", Name = "admin_roles_created_by_id_fk")]
[Index("UpdatedById", Name = "admin_roles_updated_by_id_fk")]
public class AdminRole
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("code")]
    [StringLength(255)]
    public string? Code { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<AdminPermissionsRoleLink> AdminPermissionsRoleLinks { get; set; } = new List<AdminPermissionsRoleLink>();

    [InverseProperty("Role")]
    public virtual ICollection<AdminUsersRolesLink> AdminUsersRolesLinks { get; set; } = new List<AdminUsersRolesLink>();

    [ForeignKey("CreatedById")]
    [InverseProperty("AdminRoleCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [ForeignKey("UpdatedById")]
    [InverseProperty("AdminRoleUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
