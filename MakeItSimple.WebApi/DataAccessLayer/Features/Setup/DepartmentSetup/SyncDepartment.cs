using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup
{


    public class SyncDepartmentCommand : IRequest<Result>
    {
        public ICollection<Department> Departments { get; set; }

        public Guid Modified_By { get; set; }
        public Guid? Added_By { get; set; }
        public DateTime? SyncDate { get; set; }


        public class Department
        {
            public int Department_No { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string BusinessUnit_Name {  get; set; }
            public string Sync_Status { get; set; }
            public DateTime Created_At { get; set; }
            public DateTime? Update_dAt { get; set; }

        }
    }


    public class Handler : IRequestHandler<SyncDepartmentCommand, Result>
    {
        private readonly MisDbContext _context;

        public Handler(MisDbContext context)
        {
            _context = context;
        }


        public async Task<Result> Handle(SyncDepartmentCommand command, CancellationToken cancellationToken)
        {
            var AllInputSync = new List<SyncDepartmentCommand.Department>();
            var DuplicateSync = new List<SyncDepartmentCommand.Department>();
            var AvailableSync = new List<SyncDepartmentCommand.Department>();
            var UpdateSync = new List<SyncDepartmentCommand.Department>();
            var DepartmentCodeNullOrEmpty = new List<SyncDepartmentCommand.Department>();
            var DepartmentNameNullOrEmpty = new List<SyncDepartmentCommand.Department>();
            var BusinessUnitNotExist = new List<SyncDepartmentCommand.Department>();

            foreach (SyncDepartmentCommand.Department department in command.Departments)
            {
                var businessUnitNotExist = await _context.BusinessUnits.FirstOrDefaultAsync(x => x.BusinessName == department.BusinessUnit_Name, cancellationToken);
                if (businessUnitNotExist == null)
                {
                    BusinessUnitNotExist.Add(department);
                    continue;
                }

                if (string.IsNullOrEmpty(department.Department_Code))
                {
                    DepartmentCodeNullOrEmpty.Add(department);
                    continue;
                }
                if (string.IsNullOrEmpty(department.Department_Name))
                {
                    DepartmentNameNullOrEmpty.Add(department);
                    continue;
                }


                if (command.Departments.Count(d => d.Department_Code == department.Department_Code && d.Department_Name == department.Department_Name) > 1)
                {
                    DuplicateSync.Add(department);

                }
                else
                {
                    var ExistingDepartment = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentNo == department.Department_No, cancellationToken);
                    if (ExistingDepartment != null)
                    {
                        bool hasChanged = false;

                        if (ExistingDepartment.DepartmentCode != department.Department_Code)
                        {
                            ExistingDepartment.DepartmentCode = department.Department_Code;
                            hasChanged = true;
                        }

                        if (ExistingDepartment.DepartmentName != department.Department_Name)
                        {
                            ExistingDepartment.DepartmentName = department.Department_Name;
                            hasChanged = true;
                        }


                        if (ExistingDepartment.BusinessUnitId != businessUnitNotExist.Id)
                        {
                            ExistingDepartment.BusinessUnitId = businessUnitNotExist.Id;
                            hasChanged = true;
                        }

                        if (hasChanged)
                        {
                            ExistingDepartment.UpdatedAt = DateTime.Now;
                            ExistingDepartment.ModifiedBy = command.Modified_By;
                            ExistingDepartment.SyncDate = DateTime.Now;
                            ExistingDepartment.SyncStatus = "New Update";

                             UpdateSync.Add(department);

                             _context.Departments.Update(ExistingDepartment);
                        }
                        if(!hasChanged)
                        {
                            ExistingDepartment.SyncDate = DateTime.Now;
                            ExistingDepartment.SyncStatus = "No new Update";
                        }
                    }
                    else
                    {
                        var AddDepartments = new Models.Setup.DepartmentSetup.Department
                        {

                            DepartmentNo = department.Department_No,
                            DepartmentCode = department.Department_Code,
                            DepartmentName = department.Department_Name,
                            BusinessUnitId  = businessUnitNotExist.Id,
                            CreatedAt = DateTime.Now,
                            AddedBy = command.Added_By,
                            SyncDate = DateTime.Now,
                            SyncStatus = "New Added"
                        };

                        AvailableSync.Add(department);
                        await _context.Departments.AddAsync(AddDepartments);
                    }

                }
 
                AllInputSync.Add(department);
            }

            var allInputByNo = AllInputSync.Select(x => x.Department_No);

            var allDataInput = await _context.Departments.Where(x => !allInputByNo.Contains(x.DepartmentNo)).ToListAsync();

            foreach(var department in allDataInput)
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

            var resultList = new
            {
                AvailableSync,
                UpdateSync,
                DuplicateSync,
                DepartmentCodeNullOrEmpty,
                DepartmentNameNullOrEmpty,
                BusinessUnitNotExist

            };


            if (DuplicateSync.Count == 0 && DepartmentCodeNullOrEmpty.Count == 0 && DepartmentNameNullOrEmpty.Count == 0 && BusinessUnitNotExist.Count() == 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully sync data");
            }

            return Result.Warning(resultList);

        }

    }



}
