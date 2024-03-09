using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.BusinessUnitSetup
{
    public class SyncBusinessUnit
    {

        public class SyncBusinessUnitCommand : IRequest<Result>
        {

            public ICollection<BusinessUnitResultCommand> BusinessUnit { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }

            public class BusinessUnitResultCommand
            {
                public int? Business_No { get; set; }
                public string Business_Code { get; set; }
                public string Business_Name { get; set; }
                public string Company_Name { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }
            }

        }

        public class Handler : IRequestHandler<SyncBusinessUnitCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncBusinessUnitCommand command, CancellationToken cancellationToken)
            {
                var AllInputSync = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var AvailableSync = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var UpdateSync = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var DuplicateSync = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var BusinessCodeNullOrEmpty = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var BusinessNameNullOrEmpty = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();
                var CompanyNotExist = new List<SyncBusinessUnitCommand.BusinessUnitResultCommand>();

                foreach (var business in command.BusinessUnit)
                {
                    var companyNotExist = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyName == business.Company_Name, cancellationToken);
                    if (companyNotExist == null)
                    {
                        CompanyNotExist.Add(business);
                        continue;
                    }
                    
                    if (string.IsNullOrEmpty(business.Business_Code))
                    {
                        BusinessCodeNullOrEmpty.Add(business);
                        continue;
                    }

                    if (string.IsNullOrEmpty(business.Business_Name))
                    {
                        BusinessNameNullOrEmpty.Add(business);
                        continue;
                    }

                    if (command.BusinessUnit.Count(x => x.Business_Code == business.Business_Code && x.Business_Name == business.Business_Name) > 1)
                    {
                        DuplicateSync.Add(business);
                    }
                    else
                    {
                        var ExistingBusinessTitle = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.Business_No == business.Business_No, cancellationToken);
                        if (ExistingBusinessTitle != null)
                        {
                            bool hasChanged = false;

                            if (ExistingBusinessTitle.BusinessCode != business.Business_Code)
                            {
                                ExistingBusinessTitle.BusinessCode = business.Business_Code;
                                hasChanged = true;
                            }

                            if (ExistingBusinessTitle.BusinessName != business.Business_Name)
                            {
                                ExistingBusinessTitle.BusinessName =   business.Business_Name;
                                hasChanged = true;
                            }

                            if (ExistingBusinessTitle.CompanyId != companyNotExist.Id)
                            {
                                ExistingBusinessTitle.CompanyId = companyNotExist.Id;
                                hasChanged = true;
                            }

                            if (hasChanged)
                            {
                                ExistingBusinessTitle.UpdatedAt = DateTime.Now;
                                ExistingBusinessTitle.ModifiedBy = command.Modified_By;
                                ExistingBusinessTitle.SyncDate = DateTime.Now;
                                ExistingBusinessTitle.SyncStatus = "New Update";

                                UpdateSync.Add(business);

                            }
                            if (!hasChanged)
                            {
                                ExistingBusinessTitle.SyncDate = DateTime.Now;
                                ExistingBusinessTitle.SyncStatus = "No new Update";
                            }

                        }
                        else
                        {
                            var AddBusinesssUnit = new BusinessUnit
                            {
                                Business_No = business.Business_No,
                                BusinessName = business.Business_Name,
                                BusinessCode = business.Business_Code,
                                CompanyId = companyNotExist.Id,
                                CreatedAt = DateTime.Now,
                                AddedBy = command.Added_By,
                                SyncDate = DateTime.Now,
                                SyncStatus = "New Added"

                            };

                            AvailableSync.Add(business);
                            await _context.BusinessUnits.AddAsync(AddBusinesssUnit);
                        }

                    }
                    AllInputSync.Add(business);
                }

                var allInputByNo = AllInputSync.Select(x => x.Business_No);

                var allDataInput = await _context.BusinessUnits.Where(x => !allInputByNo.Contains(x.Business_No)).ToListAsync();

                foreach (var business in allDataInput)
                {
                    business.IsActive = false;
                    var departmentList = await _context.Departments.Where(x => x.BusinessUnitId == business.Id).ToListAsync();

                    foreach (var department in departmentList)
                    {
                        department.IsActive = false;

                        var channelList = await _context.Channels.Where(x => x.DepartmentId == department.Id).ToListAsync();

                        foreach (var channels in channelList)
                        {
                            channels.IsActive = false;

                            var channelUserList = await _context.ChannelUsers.Where(x => x.ChannelId == channels.Id).ToListAsync();

                            var ApproverSetupList = await _context.Approvers.Where(x => x.ChannelId == channels.Id).ToListAsync();

                            foreach (var channelUsers in channelUserList)
                            {
                                channelUsers.IsActive = false;
                            }

                            foreach (var approver in ApproverSetupList)
                            {
                                approver.IsActive = false;
                            }

                        }



                        var UnitList = await _context.Units.Where(x => x.DepartmentId == department.Id).ToListAsync();

                        foreach (var units in UnitList)
                        {
                            units.IsActive = false;

                            var subUnitsList = await _context.SubUnits.Where(x => x.UnitId == units.Id).ToListAsync();

                            foreach (var subUnit in subUnitsList)
                            {
                                subUnit.IsActive = false;


                            }
                        }
                    }

                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    BusinessCodeNullOrEmpty,
                    BusinessNameNullOrEmpty,
                    CompanyNotExist

                };


                if (DuplicateSync.Count == 0 && CompanyNotExist.Count == 0 && BusinessCodeNullOrEmpty.Count == 0 && BusinessNameNullOrEmpty.Count == 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Warning(resultList);
            }
        }


    }


}
