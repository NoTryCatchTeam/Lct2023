using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("up_roles")]
[Index("CreatedById", Name = "up_roles_created_by_id_fk")]
[Index("UpdatedById", Name = "up_roles_updated_by_id_fk")]
public class UpRole
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

    [Column("type")]
    [StringLength(255)]
    public string? Type { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("UpRoleCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<UpPermissionsRoleLink> UpPermissionsRoleLinks { get; set; } = new List<UpPermissionsRoleLink>();

    [InverseProperty("Role")]
    public virtual ICollection<UpUsersRoleLink> UpUsersRoleLinks { get; set; } = new List<UpUsersRoleLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("UpRoleUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
