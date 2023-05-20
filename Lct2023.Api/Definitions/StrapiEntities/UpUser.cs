using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("up_users")]
[Index("CreatedById", Name = "up_users_created_by_id_fk")]
[Index("UpdatedById", Name = "up_users_updated_by_id_fk")]
public class UpUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(255)]
    public string? Username { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("provider")]
    [StringLength(255)]
    public string? Provider { get; set; }

    [Column("password")]
    [StringLength(255)]
    public string? Password { get; set; }

    [Column("reset_password_token")]
    [StringLength(255)]
    public string? ResetPasswordToken { get; set; }

    [Column("confirmation_token")]
    [StringLength(255)]
    public string? ConfirmationToken { get; set; }

    [Column("confirmed")]
    public bool? Confirmed { get; set; }

    [Column("blocked")]
    public bool? Blocked { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("UpUserCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UpUsersRoleLink> UpUsersRoleLinks { get; set; } = new List<UpUsersRoleLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("UpUserUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
