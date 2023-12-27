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
            var DuplicateSync = new List<SyncDepartmentCommand.Department>();
            var AvailableSync = new List<SyncDepartmentCommand.Department>();
            var UpdateSync = new List<SyncDepartmentCommand.Department>();
            var DepartmentCodeNullOrEmpty = new List<SyncDepartmentCommand.Department>();
            var DepartmentNameNullOrEmpty = new List<SyncDepartmentCommand.Department>();

            foreach (SyncDepartmentCommand.Department department in command.Departments)
            {




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


                if (command.Departments.Count(d => d.Department_No == department.Department_No && d.Department_Name == department.Department_Name) > 1)
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
                            CreatedAt = DateTime.Now,
                            AddedBy = command.Added_By,
                            SyncDate = DateTime.Now,
                            SyncStatus = "New Added"
                        };

                        AvailableSync.Add(department);         
                        await _context.Departments.AddAsync(AddDepartments);
                    }

                }

            }

            var resultList = new
            {
                AvailableSync,
                UpdateSync,
                DuplicateSync,
                DepartmentCodeNullOrEmpty,
                DepartmentNameNullOrEmpty

            };


            if (DuplicateSync.Count == 0 && DepartmentCodeNullOrEmpty.Count == 0 && DepartmentNameNullOrEmpty.Count == 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully sync data");
            }

            return Result.Success(resultList);

        }

    }



}
