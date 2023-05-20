using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("upload_folders_parent_links")]
[Index("FolderId", Name = "upload_folders_parent_links_fk")]
[Index("InvFolderId", Name = "upload_folders_parent_links_inv_fk")]
[Index("FolderOrder", Name = "upload_folders_parent_links_order_inv_fk")]
[Index("FolderId", "InvFolderId", Name = "upload_folders_parent_links_unique", IsUnique = true)]
public class UploadFoldersParentLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("folder_id")]
    public int? FolderId { get; set; }

    [Column("inv_folder_id")]
    public int? InvFolderId { get; set; }

    [Column("folder_order")]
    public double? FolderOrder { get; set; }

    [ForeignKey("FolderId")]
    [InverseProperty("UploadFoldersParentLinkFolders")]
    public virtual UploadFolder? Folder { get; set; }

    [ForeignKey("InvFolderId")]
    [InverseProperty("UploadFoldersParentLinkInvFolders")]
    public virtual UploadFolder? InvFolder { get; set; }
}
