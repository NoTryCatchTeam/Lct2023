using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("quizzes_art_category_links")]
[Index("QuizId", Name = "quizzes_art_category_links_fk")]
[Index("ArtCategoryId", Name = "quizzes_art_category_links_inv_fk")]
[Index("QuizId", "ArtCategoryId", Name = "quizzes_art_category_links_unique", IsUnique = true)]
public class QuizzesArtCategoryLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("quiz_id")]
    public int? QuizId { get; set; }

    [Column("art_category_id")]
    public int? ArtCategoryId { get; set; }

    [ForeignKey("ArtCategoryId")]
    [InverseProperty("QuizzesArtCategoryLinks")]
    public virtual ArtCategory? ArtCategory { get; set; }

    [ForeignKey("QuizId")]
    [InverseProperty("QuizzesArtCategoryLinks")]
    public virtual Quiz? Quiz { get; set; }
}
