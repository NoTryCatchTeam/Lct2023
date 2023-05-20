using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("files")]
[Index("CreatedById", Name = "files_created_by_id_fk")]
[Index("UpdatedById", Name = "files_updated_by_id_fk")]
[Index("CreatedAt", Name = "upload_files_created_at_index")]
[Index("Ext", Name = "upload_files_ext_index")]
[Index("FolderPath", Name = "upload_files_folder_path_index")]
[Index("Name", Name = "upload_files_name_index")]
[Index("Size", Name = "upload_files_size_index")]
[Index("UpdatedAt", Name = "upload_files_updated_at_index")]
public class CmsFile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("alternative_text")]
    [StringLength(255)]
    public string? AlternativeText { get; set; }

    [Column("caption")]
    [StringLength(255)]
    public string? Caption { get; set; }

    [Column("width")]
    public int? Width { get; set; }

    [Column("height")]
    public int? Height { get; set; }

    [Column("formats", TypeName = "jsonb")]
    public string? Formats { get; set; }

    [Column("hash")]
    [StringLength(255)]
    public string? Hash { get; set; }

    [Column("ext")]
    [StringLength(255)]
    public string? Ext { get; set; }

    [Column("mime")]
    [StringLength(255)]
    public string? Mime { get; set; }

    [Column("size")]
    [Precision(10, 2)]
    public decimal? Size { get; set; }

    [Column("url")]
    [StringLength(255)]
    public string? Url { get; set; }

    [Column("preview_url")]
    [StringLength(255)]
    public string? PreviewUrl { get; set; }

    [Column("provider")]
    [StringLength(255)]
    public string? Provider { get; set; }

    [Column("provider_metadata", TypeName = "jsonb")]
    public string? ProviderMetadata { get; set; }

    [Column("folder_path")]
    [StringLength(255)]
    public string? FolderPath { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("FileCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<FilesFolderLink> FilesFolderLinks { get; set; } = new List<FilesFolderLink>();

    [InverseProperty("File")]
    public virtual ICollection<FilesRelatedMorph> FilesRelatedMorphs { get; set; } = new List<FilesRelatedMorph>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("FileUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
