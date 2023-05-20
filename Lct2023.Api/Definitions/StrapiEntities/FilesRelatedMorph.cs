using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("files_related_morphs")]
[Index("FileId", Name = "files_related_morphs_fk")]
[Index("RelatedId", Name = "files_related_morphs_id_column_index")]
[Index("Order", Name = "files_related_morphs_order_index")]
public class FilesRelatedMorph
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("file_id")]
    public int? FileId { get; set; }

    [Column("related_id")]
    public int? RelatedId { get; set; }

    [Column("related_type")]
    [StringLength(255)]
    public string? RelatedType { get; set; }

    [Column("field")]
    [StringLength(255)]
    public string? Field { get; set; }

    [Column("order")]
    public double? Order { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("FilesRelatedMorphs")]
    public virtual CmsFile? File { get; set; }
}
