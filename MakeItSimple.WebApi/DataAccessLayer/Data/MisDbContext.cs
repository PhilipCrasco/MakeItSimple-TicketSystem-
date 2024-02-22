using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ApproverSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ChannelSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CompanySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.DepartmentSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.LocationSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.UnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace MakeItSimple.WebApi.DataAccessLayer.Data
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<AccountTitle> AccountTitles { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<SubUnit> SubUnits { get; set; }
        public virtual DbSet<Channel> Channels {  get; set; }
        public virtual DbSet<ChannelUser> ChannelUsers { get; set; }
        public virtual DbSet<Category> Categories {  get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Approver> Approvers { get; set; }
        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }
        public virtual DbSet<TicketConcern> TicketConcerns { get; set; }
        public virtual DbSet<RequestGenerator> RequestGenerators { get; set; }
        public virtual DbSet<ClosingTAttachment> ClosingTAttachments { get; set; }
        public virtual DbSet<TransferTicketConcern> TransferTicketConcerns { get; set; }
        
        public virtual DbSet<ReTicketConcern> ReTicketConcerns { get; set; }

        public virtual DbSet<ApproverTicketing> ApproverTicketings { get; set; }
        public virtual DbSet<ClosingGenerator> ClosingGenerators { get; set; }

        public virtual DbSet<TicketHistory> TicketHistories { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new BusinessUnitConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTitleConfiguration());
            modelBuilder.ApplyConfiguration(new SubUnitConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ClosingTAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TransferTicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverTicketingConfiguration());  
            modelBuilder.ApplyConfiguration(new ReTicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new TicketHistoryConfiguration());

        }


    }
}
