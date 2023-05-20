using System;
using System.Collections.Generic;
using Lct2023.Api.Definitions.StrapiEntities;
using Microsoft.EntityFrameworkCore;

namespace Lct2023.Api.Dal;

// Generated DB context, use NpgsqlDbContext
public class Lct2023Context : DbContext
{
    public virtual DbSet<AdminPermission> AdminPermissions { get; set; }

    public virtual DbSet<AdminPermissionsRoleLink> AdminPermissionsRoleLinks { get; set; }

    public virtual DbSet<AdminRole> AdminRoles { get; set; }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<AdminUsersRolesLink> AdminUsersRolesLinks { get; set; }

    public virtual DbSet<ArtCategory> ArtCategories { get; set; }

    public virtual DbSet<CmsFile> Files { get; set; }

    public virtual DbSet<FilesFolderLink> FilesFolderLinks { get; set; }

    public virtual DbSet<FilesRelatedMorph> FilesRelatedMorphs { get; set; }

    public virtual DbSet<I18nLocale> I18nLocales { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizzesArtCategoryLink> QuizzesArtCategoryLinks { get; set; }

    public virtual DbSet<StrapiApiToken> StrapiApiTokens { get; set; }

    public virtual DbSet<StrapiApiTokenPermission> StrapiApiTokenPermissions { get; set; }

    public virtual DbSet<StrapiApiTokenPermissionsTokenLink> StrapiApiTokenPermissionsTokenLinks { get; set; }

    public virtual DbSet<StrapiCoreStoreSetting> StrapiCoreStoreSettings { get; set; }

    public virtual DbSet<StrapiDatabaseSchema> StrapiDatabaseSchemas { get; set; }

    public virtual DbSet<StrapiMigration> StrapiMigrations { get; set; }

    public virtual DbSet<StrapiTransferToken> StrapiTransferTokens { get; set; }

    public virtual DbSet<StrapiTransferTokenPermission> StrapiTransferTokenPermissions { get; set; }

    public virtual DbSet<StrapiTransferTokenPermissionsTokenLink> StrapiTransferTokenPermissionsTokenLinks { get; set; }

    public virtual DbSet<StrapiWebhook> StrapiWebhooks { get; set; }

    public virtual DbSet<UpPermission> UpPermissions { get; set; }

    public virtual DbSet<UpPermissionsRoleLink> UpPermissionsRoleLinks { get; set; }

    public virtual DbSet<UpRole> UpRoles { get; set; }

    public virtual DbSet<UpUser> UpUsers { get; set; }

    public virtual DbSet<UpUsersRoleLink> UpUsersRoleLinks { get; set; }

    public virtual DbSet<UploadFolder> UploadFolders { get; set; }

    public virtual DbSet<UploadFoldersParentLink> UploadFoldersParentLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_permissions_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.AdminPermissionCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_permissions_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.AdminPermissionUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_permissions_updated_by_id_fk");
        });

        modelBuilder.Entity<AdminPermissionsRoleLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_permissions_role_links_pkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.AdminPermissionsRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admin_permissions_role_links_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.AdminPermissionsRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admin_permissions_role_links_inv_fk");
        });

        modelBuilder.Entity<AdminRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_roles_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.AdminRoleCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_roles_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.AdminRoleUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_roles_updated_by_id_fk");
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_users_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.InverseCreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_users_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.InverseUpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("admin_users_updated_by_id_fk");
        });

        modelBuilder.Entity<AdminUsersRolesLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("admin_users_roles_links_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.AdminUsersRolesLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admin_users_roles_links_inv_fk");

            entity.HasOne(d => d.User).WithMany(p => p.AdminUsersRolesLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("admin_users_roles_links_fk");
        });

        modelBuilder.Entity<ArtCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("art_categories_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.ArtCategoryCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("art_categories_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.ArtCategoryUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("art_categories_updated_by_id_fk");
        });

        modelBuilder.Entity<CmsFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.FileCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("files_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.FileUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("files_updated_by_id_fk");
        });

        modelBuilder.Entity<FilesFolderLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_folder_links_pkey");

            entity.HasOne(d => d.File).WithMany(p => p.FilesFolderLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("files_folder_links_fk");

            entity.HasOne(d => d.Folder).WithMany(p => p.FilesFolderLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("files_folder_links_inv_fk");
        });

        modelBuilder.Entity<FilesRelatedMorph>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_related_morphs_pkey");

            entity.HasOne(d => d.File).WithMany(p => p.FilesRelatedMorphs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("files_related_morphs_fk");
        });

        modelBuilder.Entity<I18nLocale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("i18n_locale_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.I18nLocaleCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("i18n_locale_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.I18nLocaleUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("i18n_locale_updated_by_id_fk");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("locations_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.LocationCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("locations_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.LocationUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("locations_updated_by_id_fk");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quizzes_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.QuizCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("quizzes_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.QuizUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("quizzes_updated_by_id_fk");
        });

        modelBuilder.Entity<QuizzesArtCategoryLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quizzes_art_category_links_pkey");

            entity.HasOne(d => d.ArtCategory).WithMany(p => p.QuizzesArtCategoryLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("quizzes_art_category_links_inv_fk");

            entity.HasOne(d => d.Quiz).WithMany(p => p.QuizzesArtCategoryLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("quizzes_art_category_links_fk");
        });

        modelBuilder.Entity<StrapiApiToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_api_tokens_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.StrapiApiTokenCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_api_tokens_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.StrapiApiTokenUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_api_tokens_updated_by_id_fk");
        });

        modelBuilder.Entity<StrapiApiTokenPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_api_token_permissions_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.StrapiApiTokenPermissionCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_api_token_permissions_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.StrapiApiTokenPermissionUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_api_token_permissions_updated_by_id_fk");
        });

        modelBuilder.Entity<StrapiApiTokenPermissionsTokenLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_api_token_permissions_token_links_pkey");

            entity.HasOne(d => d.ApiToken).WithMany(p => p.StrapiApiTokenPermissionsTokenLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("strapi_api_token_permissions_token_links_inv_fk");

            entity.HasOne(d => d.ApiTokenPermission).WithMany(p => p.StrapiApiTokenPermissionsTokenLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("strapi_api_token_permissions_token_links_fk");
        });

        modelBuilder.Entity<StrapiCoreStoreSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_core_store_settings_pkey");
        });

        modelBuilder.Entity<StrapiDatabaseSchema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_database_schema_pkey");
        });

        modelBuilder.Entity<StrapiMigration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_migrations_pkey");
        });

        modelBuilder.Entity<StrapiTransferToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_transfer_tokens_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.StrapiTransferTokenCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_transfer_tokens_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.StrapiTransferTokenUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_transfer_tokens_updated_by_id_fk");
        });

        modelBuilder.Entity<StrapiTransferTokenPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_transfer_token_permissions_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.StrapiTransferTokenPermissionCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_transfer_token_permissions_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.StrapiTransferTokenPermissionUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("strapi_transfer_token_permissions_updated_by_id_fk");
        });

        modelBuilder.Entity<StrapiTransferTokenPermissionsTokenLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_transfer_token_permissions_token_links_pkey");

            entity.HasOne(d => d.TransferToken).WithMany(p => p.StrapiTransferTokenPermissionsTokenLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("strapi_transfer_token_permissions_token_links_inv_fk");

            entity.HasOne(d => d.TransferTokenPermission).WithMany(p => p.StrapiTransferTokenPermissionsTokenLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("strapi_transfer_token_permissions_token_links_fk");
        });

        modelBuilder.Entity<StrapiWebhook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("strapi_webhooks_pkey");
        });

        modelBuilder.Entity<UpPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("up_permissions_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UpPermissionCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_permissions_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.UpPermissionUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_permissions_updated_by_id_fk");
        });

        modelBuilder.Entity<UpPermissionsRoleLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("up_permissions_role_links_pkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.UpPermissionsRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("up_permissions_role_links_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.UpPermissionsRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("up_permissions_role_links_inv_fk");
        });

        modelBuilder.Entity<UpRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("up_roles_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UpRoleCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_roles_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.UpRoleUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_roles_updated_by_id_fk");
        });

        modelBuilder.Entity<UpUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("up_users_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UpUserCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_users_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.UpUserUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("up_users_updated_by_id_fk");
        });

        modelBuilder.Entity<UpUsersRoleLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("up_users_role_links_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.UpUsersRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("up_users_role_links_inv_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UpUsersRoleLinks)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("up_users_role_links_fk");
        });

        modelBuilder.Entity<UploadFolder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("upload_folders_pkey");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.UploadFolderCreatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("upload_folders_created_by_id_fk");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.UploadFolderUpdatedBies)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("upload_folders_updated_by_id_fk");
        });

        modelBuilder.Entity<UploadFoldersParentLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("upload_folders_parent_links_pkey");

            entity.HasOne(d => d.Folder).WithMany(p => p.UploadFoldersParentLinkFolders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("upload_folders_parent_links_fk");

            entity.HasOne(d => d.InvFolder).WithMany(p => p.UploadFoldersParentLinkInvFolders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("upload_folders_parent_links_inv_fk");
        });
    }
}
