using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("upload_folders")]
[Index("CreatedById", Name = "upload_folders_created_by_id_fk")]
[Index("PathId", Name = "upload_folders_path_id_index", IsUnique = true)]
[Index("Path", Name = "upload_folders_path_index", IsUnique = true)]
[Index("UpdatedById", Name = "upload_folders_updated_by_id_fk")]
public class UploadFolder
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("path_id")]
    public int? PathId { get; set; }

    [Column("path")]
    [StringLength(255)]
    public string? Path { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("UploadFolderCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("Folder")]
    public virtual ICollection<FilesFolderLink> FilesFolderLinks { get; set; } = new List<FilesFolderLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("UploadFolderUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }

    [InverseProperty("Folder")]
    public virtual ICollection<UploadFoldersParentLink> UploadFoldersParentLinkFolders { get; set; } = new List<UploadFoldersParentLink>();

    [InverseProperty("InvFolder")]
    public virtual ICollection<UploadFoldersParentLink> UploadFoldersParentLinkInvFolders { get; set; } = new List<UploadFoldersParentLink>();
}
