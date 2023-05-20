using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Definitions.StrapiEntities;

[Table("strapi_transfer_token_permissions_token_links")]
[Index("TransferTokenPermissionId", Name = "strapi_transfer_token_permissions_token_links_fk")]
[Index("TransferTokenId", Name = "strapi_transfer_token_permissions_token_links_inv_fk")]
[Index("TransferTokenPermissionOrder", Name = "strapi_transfer_token_permissions_token_links_order_inv_fk")]
[Index("TransferTokenPermissionId", "TransferTokenId", Name = "strapi_transfer_token_permissions_token_links_unique", IsUnique = true)]
public class StrapiTransferTokenPermissionsTokenLink
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("transfer_token_permission_id")]
    public int? TransferTokenPermissionId { get; set; }

    [Column("transfer_token_id")]
    public int? TransferTokenId { get; set; }

    [Column("transfer_token_permission_order")]
    public double? TransferTokenPermissionOrder { get; set; }

    [ForeignKey("TransferTokenId")]
    [InverseProperty("StrapiTransferTokenPermissionsTokenLinks")]
    public virtual StrapiTransferToken? TransferToken { get; set; }

    [ForeignKey("TransferTokenPermissionId")]
    [InverseProperty("StrapiTransferTokenPermissionsTokenLinks")]
    public virtual StrapiTransferTokenPermission? TransferTokenPermission { get; set; }
}
