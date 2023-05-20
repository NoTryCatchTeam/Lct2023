using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_api_token_permissions_token_links")]
[Index("ApiTokenPermissionId", Name = "strapi_api_token_permissions_token_links_fk")]
[Index("ApiTokenId", Name = "strapi_api_token_permissions_token_links_inv_fk")]
[Index("ApiTokenPermissionOrder", Name = "strapi_api_token_permissions_token_links_order_inv_fk")]
[Index("ApiTokenPermissionId", "ApiTokenId", Name = "strapi_api_token_permissions_token_links_unique", IsUnique = true)]
public class StrapiApiTokenPermissionsTokenLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("api_token_permission_id")]
    public int? ApiTokenPermissionId { get; set; }

    [Column("api_token_id")]
    public int? ApiTokenId { get; set; }

    [Column("api_token_permission_order")]
    public double? ApiTokenPermissionOrder { get; set; }

    [ForeignKey("ApiTokenId")]
    [InverseProperty("StrapiApiTokenPermissionsTokenLinks")]
    public virtual StrapiApiToken? ApiToken { get; set; }

    [ForeignKey("ApiTokenPermissionId")]
    [InverseProperty("StrapiApiTokenPermissionsTokenLinks")]
    public virtual StrapiApiTokenPermission? ApiTokenPermission { get; set; }
}
