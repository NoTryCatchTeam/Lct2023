using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("i18n_locale")]
[Index("CreatedById", Name = "i18n_locale_created_by_id_fk")]
[Index("UpdatedById", Name = "i18n_locale_updated_by_id_fk")]
public class I18nLocale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("code")]
    [StringLength(255)]
    public string? Code { get; set; }

    [Column("created_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6) without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("I18nLocaleCreatedBies")]
    public virtual AdminUser? CreatedBy { get; set; }

    [ForeignKey("UpdatedById")]
    [InverseProperty("I18nLocaleUpdatedBies")]
    public virtual AdminUser? UpdatedBy { get; set; }
}
