using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("quizzes")]
[Index("CreatedById", Name = "quizzes_created_by_id_fk")]
[Index("UpdatedById", Name = "quizzes_updated_by_id_fk")]
public class Quiz
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("question")]
    public string? Question { get; set; }

    [Column("a")]
    public string? A { get; set; }

    [Column("b")]
    public string? B { get; set; }

    [Column("c")]
    public string? C { get; set; }

    [Column("d")]
    public string? D { get; set; }

    [Column("answer")]
    public string? Answer { get; set; }

    [Column("difficulty")]
    [StringLength(255)]
    public string? Difficulty { get; set; }

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

    [Column("explanation")]
    public string? Explanation { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("QuizCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [InverseProperty("Quiz")]
    public virtual ICollection<QuizzesArtCategoryLink> QuizzesArtCategoryLinks { get; set; } = new List<QuizzesArtCategoryLink>();

    [ForeignKey("UpdatedById")]
    [InverseProperty("QuizUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
