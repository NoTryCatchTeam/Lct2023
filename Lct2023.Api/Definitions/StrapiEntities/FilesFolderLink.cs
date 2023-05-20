using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("files_folder_links")]
[Index("FileId", Name = "files_folder_links_fk")]
[Index("FolderId", Name = "files_folder_links_inv_fk")]
[Index("FileOrder", Name = "files_folder_links_order_inv_fk")]
[Index("FileId", "FolderId", Name = "files_folder_links_unique", IsUnique = true)]
public class FilesFolderLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("file_id")]
    public int? FileId { get; set; }

    [Column("folder_id")]
    public int? FolderId { get; set; }

    [Column("file_order")]
    public double? FileOrder { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("FilesFolderLinks")]
    public virtual CmsFile? File { get; set; }

    [ForeignKey("FolderId")]
    [InverseProperty("FilesFolderLinks")]
    public virtual UploadFolder? Folder { get; set; }
}
