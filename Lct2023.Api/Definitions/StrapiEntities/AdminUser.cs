using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("admin_users")]
[Index("CreatedById", Name = "admin_users_created_by_id_fk")]
[Index("UpdatedById", Name = "admin_users_updated_by_id_fk")]
public class AdminUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("firstname")]
    [StringLength(255)]
    public string? Firstname { get; set; }

    [Column("lastname")]
    [StringLength(255)]
    public string? Lastname { get; set; }

    [Column("username")]
    [StringLength(255)]
    public string? Username { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("password")]
    [StringLength(255)]
    public string? Password { get; set; }

    [Column("reset_password_token")]
    [StringLength(255)]
    public string? ResetPasswordToken { get; set; }

    [Column("registration_token")]
    [StringLength(255)]
    public string? RegistrationToken { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("blocked")]
    public bool? Blocked { get; set; }

    [Column("prefered_language")]
    [StringLength(255)]
    public string? PreferedLanguage { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [InverseProperty("CreatedBy")]
    public virtual ICollection<AdminPermission> AdminPermissionCreatedBies { get; set; } = new List<AdminPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<AdminPermission> AdminPermissionUpdatedBies { get; set; } = new List<AdminPermission>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<AdminRole> AdminRoleCreatedBies { get; set; } = new List<AdminRole>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<AdminRole> AdminRoleUpdatedBies { get; set; } = new List<AdminRole>();

    [InverseProperty("User")]
    public virtual ICollection<AdminUsersRolesLink> AdminUsersRolesLinks { get; set; } = new List<AdminUsersRolesLink>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<ArtCategory> ArtCategoryCreatedBies { get; set; } = new List<ArtCategory>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<ArtCategory> ArtCategoryUpdatedBies { get; set; } = new List<ArtCategory>();

    [ForeignKey("CreatedById")]
    [InverseProperty("InverseCreatedBy")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("CreatedBy")]
    public virtual ICollection<CmsFile> FileCreatedBies { get; set; } = new List<CmsFile>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<CmsFile> FileUpdatedBies { get; set; } = new List<CmsFile>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<I18nLocale> I18nLocaleCreatedBies { get; set; } = new List<I18nLocale>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<I18nLocale> I18nLocaleUpdatedBies { get; set; } = new List<I18nLocale>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<AdminUser> InverseCreatedBy { get; set; } = new List<AdminUser>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<AdminUser> InverseUpdatedBy { get; set; } = new List<AdminUser>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<Location> LocationCreatedBies { get; set; } = new List<Location>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<Location> LocationUpdatedBies { get; set; } = new List<Location>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<Quiz> QuizCreatedBies { get; set; } = new List<Quiz>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<Quiz> QuizUpdatedBies { get; set; } = new List<Quiz>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<StrapiApiToken> StrapiApiTokenCreatedBies { get; set; } = new List<StrapiApiToken>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<StrapiApiTokenPermission> StrapiApiTokenPermissionCreatedBies { get; set; } = new List<StrapiApiTokenPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<StrapiApiTokenPermission> StrapiApiTokenPermissionUpdatedBies { get; set; } = new List<StrapiApiTokenPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<StrapiApiToken> StrapiApiTokenUpdatedBies { get; set; } = new List<StrapiApiToken>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<StrapiTransferToken> StrapiTransferTokenCreatedBies { get; set; } = new List<StrapiTransferToken>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<StrapiTransferTokenPermission> StrapiTransferTokenPermissionCreatedBies { get; set; } = new List<StrapiTransferTokenPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<StrapiTransferTokenPermission> StrapiTransferTokenPermissionUpdatedBies { get; set; } = new List<StrapiTransferTokenPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<StrapiTransferToken> StrapiTransferTokenUpdatedBies { get; set; } = new List<StrapiTransferToken>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<UpPermission> UpPermissionCreatedBies { get; set; } = new List<UpPermission>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<UpPermission> UpPermissionUpdatedBies { get; set; } = new List<UpPermission>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<UpRole> UpRoleCreatedBies { get; set; } = new List<UpRole>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<UpRole> UpRoleUpdatedBies { get; set; } = new List<UpRole>();

    [InverseProperty("CreatedBy")]
    public virtual ICollection<UpUser> UpUserCreatedBies { get; set; } = new List<UpUser>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<UpUser> UpUserUpdatedBies { get; set; } = new List<UpUser>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("InverseUpdatedBy")]
    public virtual AdminUser? UpdatedBy { get; set; }

    [InverseProperty("CreatedBy")]
    public virtual ICollection<UploadFolder> UploadFolderCreatedBies { get; set; } = new List<UploadFolder>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<UploadFolder> UploadFolderUpdatedBies { get; set; } = new List<UploadFolder>();
}
