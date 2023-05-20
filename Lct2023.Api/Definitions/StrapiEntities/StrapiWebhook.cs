using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_webhooks")]
public class StrapiWebhook
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("url")]
    public string? Url { get; set; }

    [Column("headers", TypeName = "jsonb")]
    public string? Headers { get; set; }

    [Column("events", TypeName = "jsonb")]
    public string? Events { get; set; }

    [Column("enabled")]
    public bool? Enabled { get; set; }
}
