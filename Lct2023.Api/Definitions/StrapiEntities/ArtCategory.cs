using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("art_categories")]
[Index("CreatedById", Name = "art_categories_created_by_id_fk")]
[Index("UpdatedById", Name = "art_categories_updated_by_id_fk")]
public class ArtCategory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("published_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? PublishedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("ArtCategoryCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("ArtCategory")]
    public virtual ICollection<QuizzesArtCategoryLink> QuizzesArtCategoryLinks { get; set; } = new List<QuizzesArtCategoryLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("ArtCategoryUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
