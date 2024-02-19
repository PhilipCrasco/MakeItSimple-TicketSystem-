using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.BusinessUnitSetup.SyncBusinessUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.SyncSubUnit.SyncSubUnitCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class SyncSubUnit
    {
        public class SyncSubUnitCommand : IRequest<Result>
        {
            public ICollection<SyncSubUnitsResult> SyncSubUnitsResults { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }


            public class SyncSubUnitsResult
            {
                public int SubUnit_No { get; set; }
                public string SubUnit_Code { get; set; }
                public string SubUnit_Name { get; set; }
                public string Department_Name { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }

            }
        }

        public class Handler : IRequestHandler<SyncSubUnitCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncSubUnitCommand command, CancellationToken cancellationToken)
            {
                var AllInputSync = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var AvailableSync = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var UpdateSync = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var DuplicateSync = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var SubUnitCodeNullOrEmpty = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var SubUnitNameNullOrEmpty = new List<SyncSubUnitCommand.SyncSubUnitsResult>();
                var DepartmentNotExist = new List<SyncSubUnitCommand.SyncSubUnitsResult>();

                foreach (var subUnit in command.SyncSubUnitsResults)
                {
                    var departmentNotExist = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentName == subUnit.Department_Name , cancellationToken);
                    if (departmentNotExist == null)
                    {
                        DepartmentNotExist.Add(subUnit);
                        continue;
                    }

                    if (string.IsNullOrEmpty(subUnit.SubUnit_Code))
                    {
                        SubUnitCodeNullOrEmpty.Add(subUnit);
                        continue;
                    }

                    if (string.IsNullOrEmpty(subUnit.SubUnit_Name))
                    {
                        SubUnitNameNullOrEmpty.Add(subUnit);
                        continue;
                    }

                    if (command.SyncSubUnitsResults.Count(x => x.SubUnit_Code == subUnit.SubUnit_Code && x.SubUnit_Name == subUnit.SubUnit_Name) > 1)
                    {
                        DuplicateSync.Add(subUnit);
                    }
                    else
                    {
                        var ExistingSubUnit = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitNo == subUnit.SubUnit_No, cancellationToken);
                        if (ExistingSubUnit != null)
                        {
                            bool hasChanged = false;

                            if (ExistingSubUnit.SubUnitCode != subUnit.SubUnit_Code)
                            {
                                ExistingSubUnit.SubUnitCode = subUnit.SubUnit_Code;
                                hasChanged = true;
                            }

                            if (ExistingSubUnit.SubUnitName != subUnit.SubUnit_Name)
                            {
                                ExistingSubUnit.SubUnitName = subUnit.SubUnit_Name;
                                hasChanged = true;
                            }

                            if (ExistingSubUnit.DepartmentId != departmentNotExist.Id)
                            {
                                ExistingSubUnit.DepartmentId = departmentNotExist.Id;
                                hasChanged = true;
                            }

                            if (hasChanged)
                            {
                                ExistingSubUnit.UpdatedAt = DateTime.Now;
                                ExistingSubUnit.ModifiedBy = command.Modified_By;
                                ExistingSubUnit.SyncDate = DateTime.Now;
                                ExistingSubUnit.SyncStatus = "New Update";

                                UpdateSync.Add(subUnit);

                            }
                            if (!hasChanged)
                            {
                                ExistingSubUnit.SyncDate = DateTime.Now;
                                ExistingSubUnit.SyncStatus = "No new Update";
                            }

                        }
                        else
                        {
                            var AddSubUnitUnit = new SubUnit
                            {
                                SubUnitNo = subUnit.SubUnit_No,
                                SubUnitCode = subUnit.SubUnit_Code,
                                SubUnitName = subUnit.SubUnit_Name,
                                DepartmentId = departmentNotExist.Id,
                                CreatedAt = DateTime.Now,
                                AddedBy = command.Added_By,
                                SyncDate = DateTime.Now,
                                SyncStatus = "New Added"

                            };

                            AvailableSync.Add(subUnit);
                            await _context.SubUnits.AddAsync(AddSubUnitUnit);
                        }

                    }
                    AllInputSync.Add(subUnit);
                }

                var allInputByNo = AllInputSync.Select(x => x.SubUnit_No);

                var allDataInput = await _context.SubUnits.Where(x => !allInputByNo.Contains(x.SubUnitNo)).ToListAsync();

                foreach (var account in allDataInput)
                {
                    _context.Remove(account);
                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    SubUnitCodeNullOrEmpty,
                    SubUnitNameNullOrEmpty,
                    DepartmentNotExist

                };


                if (DuplicateSync.Count == 0 && DepartmentNotExist.Count == 0 && SubUnitCodeNullOrEmpty.Count == 0 && SubUnitNameNullOrEmpty.Count == 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Success(resultList);
            }
        }






    }
}
