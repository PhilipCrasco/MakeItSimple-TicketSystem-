using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.SyncSubUnit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.UnitSetup
{
    public class SyncUnit
    {
        public class SyncUnitCommand : IRequest<Result>
        {
            public ICollection<SyncUnitsResult> SyncUnitsResults { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }


            public class SyncUnitsResult
            {
                public int Unit_No { get; set; }
                public string Unit_Code { get; set; }
                public string Unit_Name { get; set; }
                public string Department_Name { get; set; }

                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }

            }
        }

        public class Handler : IRequestHandler<SyncUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncUnitCommand command, CancellationToken cancellationToken)
            {

                var AllInputSync = new List<SyncUnitCommand.SyncUnitsResult>();
                var AvailableSync = new List<SyncUnitCommand.SyncUnitsResult>();
                var UpdateSync = new List<SyncUnitCommand.SyncUnitsResult>();
                var DuplicateSync = new List<SyncUnitCommand.SyncUnitsResult>();
                var UnitCodeNullOrEmpty = new List<SyncUnitCommand.SyncUnitsResult>();
                var UnitNameNullOrEmpty = new List<SyncUnitCommand.SyncUnitsResult>();
                var DepartmentNotExist = new List<SyncUnitCommand.SyncUnitsResult>();


                foreach (var unit in command.SyncUnitsResults)
                {
                    var departmnetNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentName == unit.Department_Name, cancellationToken);
                    if (departmnetNotExist == null)
                    {
                        DepartmentNotExist.Add(unit);
                        continue;
                    }


                    if (string.IsNullOrEmpty(unit.Unit_Code))
                    {
                        UnitCodeNullOrEmpty.Add(unit);
                        continue;
                    }

                    if (string.IsNullOrEmpty(unit.Unit_Name))
                    {
                        UnitNameNullOrEmpty.Add(unit);
                        continue;
                    }

                    if (command.SyncUnitsResults.Count(x => x.Unit_Code == unit.Unit_Name && x.Unit_Name == unit.Unit_Name) > 1)
                    {
                        DuplicateSync.Add(unit);
                    }
                    else
                    {
                        var ExistingUnit = await _context.Units.FirstOrDefaultAsync(x => x.UnitNo == unit.Unit_No, cancellationToken);
                        if (ExistingUnit != null)
                        {
                            bool hasChanged = false;

                            if (ExistingUnit.UnitCode != unit.Unit_Code)
                            {
                                ExistingUnit.UnitCode = unit.Unit_Code;
                                hasChanged = true;
                            }

                            if (ExistingUnit.UnitName != unit.Unit_Name)
                            {
                                ExistingUnit.UnitName = unit.Unit_Name;
                                hasChanged = true;
                            }

                            if (ExistingUnit.DepartmentId != departmnetNotExist.Id)
                            {
                                ExistingUnit.DepartmentId = departmnetNotExist.Id;
                                hasChanged = true;
                            }

                            if (hasChanged)
                            {
                                ExistingUnit.UpdatedAt = DateTime.Now;
                                ExistingUnit.ModifiedBy = command.Modified_By;
                                ExistingUnit.SyncDate = DateTime.Now;
                                ExistingUnit.SyncStatus = "New Update";

                                UpdateSync.Add(unit);

                            }
                            if (!hasChanged)
                            {
                                ExistingUnit.SyncDate = DateTime.Now;
                                ExistingUnit.SyncStatus = "No new Update";
                            }

                        }
                        else
                        {
                            var AddUnitUnit = new Models.Setup.UnitSetup.Unit
                            {
                                UnitNo = unit.Unit_No,
                                UnitCode = unit.Unit_Code,
                                UnitName = unit.Unit_Name,
                                DepartmentId = departmnetNotExist.Id,
                                CreatedAt = DateTime.Now,
                                AddedBy = command.Added_By,
                                SyncDate = DateTime.Now,
                                SyncStatus = "New Added"

                            };

                            AvailableSync.Add(unit);
                            await _context.Units.AddAsync(AddUnitUnit);
                        }

                    }
                    AllInputSync.Add(unit);
                }

                var allInputByNo = AllInputSync.Select(x => x.Unit_No);

                var allDataInput = await _context.Units.Where(x => !allInputByNo.Contains(x.UnitNo)).ToListAsync();

                foreach (var units in allDataInput)
                {
                    units.IsActive = false;

                    var subUnitsList = await _context.SubUnits.Where(x => x.UnitId == units.Id).ToListAsync();

                    foreach (var subUnit in subUnitsList)
                    {
                        subUnit.IsActive = false;

                        var channelList = await _context.Channels.Where(x => x.SubUnitId == subUnit.Id).ToListAsync();

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
                    }
                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    UnitCodeNullOrEmpty,
                    UnitNameNullOrEmpty,
                    DepartmentNotExist,
                };


                if (DuplicateSync.Count == 0 && DepartmentNotExist.Count == 0 && UnitCodeNullOrEmpty.Count == 0 && UnitNameNullOrEmpty.Count == 0 )
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Success(resultList);
            }
        }
    }
}
