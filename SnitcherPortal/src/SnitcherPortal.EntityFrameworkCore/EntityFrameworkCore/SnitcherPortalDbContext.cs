using SnitcherPortal.KnownProcesses;
using SnitcherPortal.Calendars;
using SnitcherPortal.SnitchingLogs;
using SnitcherPortal.ActivityRecords;
using SnitcherPortal.SupervisedComputers;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace SnitcherPortal.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class SnitcherPortalDbContext :
    AbpDbContext<SnitcherPortalDbContext>,
    IIdentityProDbContext,
    ISaasDbContext
{
    public DbSet<KnownProcess> KnownProcesses { get; set; } = null!;
    public DbSet<Calendar> Calendars { get; set; } = null!;
    public DbSet<SnitchingLog> SnitchingLogs { get; set; } = null!;
    public DbSet<ActivityRecord> ActivityRecords { get; set; } = null!;
    public DbSet<SupervisedComputer> SupervisedComputers { get; set; } = null!;
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public SnitcherPortalDbContext(DbContextOptions<SnitcherPortalDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureFeatureManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureGdpr();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(SnitcherPortalConsts.DbTablePrefix + "YourEntities", SnitcherPortalConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SnitchingLog>(b =>
            {
                b.ToTable(SnitcherPortalConsts.DbTablePrefix + "SnitchingLogs", SnitcherPortalConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Timestamp).HasColumnName(nameof(SnitchingLog.Timestamp));
                b.Property(x => x.Message).HasColumnName(nameof(SnitchingLog.Message));
                b.HasOne<SupervisedComputer>().WithMany(x => x.SnitchingLogs).HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<ActivityRecord>(b =>
            {
                b.ToTable(SnitcherPortalConsts.DbTablePrefix + "ActivityRecords", SnitcherPortalConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.StartTime).HasColumnName(nameof(ActivityRecord.StartTime));
                b.Property(x => x.EndTime).HasColumnName(nameof(ActivityRecord.EndTime));
                b.Property(x => x.Data).HasColumnName(nameof(ActivityRecord.Data));
                b.HasOne<SupervisedComputer>().WithMany(x => x.ActivityRecords).HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Calendar>(b =>
            {
                b.ToTable(SnitcherPortalConsts.DbTablePrefix + "Calendars", SnitcherPortalConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DayOfWeek).HasColumnName(nameof(Calendar.DayOfWeek));
                b.Property(x => x.AllowedHours).HasColumnName(nameof(Calendar.AllowedHours));
                b.HasOne<SupervisedComputer>().WithMany(x => x.Calendars).HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SupervisedComputer>(b =>
            {
                b.ToTable(SnitcherPortalConsts.DbTablePrefix + "SupervisedComputers", SnitcherPortalConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(SupervisedComputer.Name)).IsRequired().HasMaxLength(SupervisedComputerConsts.NameMaxLength);
                b.Property(x => x.Identifier).HasColumnName(nameof(SupervisedComputer.Identifier)).IsRequired().HasMaxLength(SupervisedComputerConsts.IdentifierMaxLength);
                b.Property(x => x.IpAddress).HasColumnName(nameof(SupervisedComputer.IpAddress)).HasMaxLength(SupervisedComputerConsts.IpAddressMaxLength);
                b.Property(x => x.IsCalendarActive).HasColumnName(nameof(SupervisedComputer.IsCalendarActive));
                b.Property(x => x.BanUntil).HasColumnName(nameof(SupervisedComputer.BanUntil));
                b.HasMany(x => x.SnitchingLogs).WithOne().HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.ActivityRecords).WithOne().HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.Calendars).WithOne().HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.KnownProcesses).WithOne().HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<KnownProcess>(b =>
            {
                b.ToTable(SnitcherPortalConsts.DbTablePrefix + "KnownProcesses", SnitcherPortalConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(KnownProcess.Name)).IsRequired().HasMaxLength(KnownProcessConsts.NameMaxLength);
                b.Property(x => x.IsHidden).HasColumnName(nameof(KnownProcess.IsHidden));
                b.Property(x => x.IsImportant).HasColumnName(nameof(KnownProcess.IsImportant));
                b.HasOne<SupervisedComputer>().WithMany(x => x.KnownProcesses).HasForeignKey(x => x.SupervisedComputerId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}