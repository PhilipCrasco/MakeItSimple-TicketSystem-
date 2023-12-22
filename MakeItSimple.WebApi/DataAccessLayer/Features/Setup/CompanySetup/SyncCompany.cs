using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.SyncCompany.SyncCompanyCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup
{
    public class SyncCompany
    {

        public class SyncCompanyCommand : IRequest<Result>
        {

            public ICollection<CompanyResultCommand> Companies { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }

            public class CompanyResultCommand
            {
                public int Company_No { get; set; }
                public string Company_Code { get; set; }
                public string Company_Name { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }
            }
        }

        public class Handler : IRequestHandler<SyncCompanyCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncCompanyCommand command, CancellationToken cancellationToken)
            {
               
                var AvailableSync = new List<SyncCompanyCommand.CompanyResultCommand>();
                var UpdateSync = new List<SyncCompanyCommand.CompanyResultCommand>();
                var DuplicateSync = new List<SyncCompanyCommand.CompanyResultCommand>();
                var CompanyCodeNullOrEmpty = new List<SyncCompanyCommand.CompanyResultCommand>();
                var CompanyNameNullOrEmpty = new List<SyncCompanyCommand.CompanyResultCommand>();

                foreach (SyncCompanyCommand.CompanyResultCommand companies in command.Companies)
                {
                    if(string.IsNullOrEmpty(companies.Company_Code))
                    {
                        CompanyCodeNullOrEmpty.Add(companies);
                        continue;
                    }

                    if (string.IsNullOrEmpty(companies.Company_Name))
                    {
                        CompanyNameNullOrEmpty.Add(companies);
                        continue;
                    }

                    if (command.Companies.Count(x => x.Company_Code == companies.Company_Code && x.Company_Name == companies.Company_Name) > 1 )
                    {
                        DuplicateSync.Add(companies);
                    }
                    else
                    {
                        var ExistingCompany = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyNo == companies.Company_No, cancellationToken);
                        if(ExistingCompany != null )
                        {
                            bool hasChanged = false;

                            if(ExistingCompany.CompanyCode != companies.Company_Code)
                            {
                                ExistingCompany.CompanyCode = companies.Company_Code;
                                hasChanged = true;
                            }

                            if (ExistingCompany.CompanyName != companies.Company_Code)
                            {
                                ExistingCompany.CompanyCode = companies.Company_Name;
                                hasChanged = true;
                            }

                            if(hasChanged)
                            {
                                ExistingCompany.UpdatedAt = DateTime.Now;
                                ExistingCompany.ModifiedBy = command.Modified_By;
                                ExistingCompany.SyncDate = DateTime.Now;
                                ExistingCompany.SyncStatus = "New Update";

                                UpdateSync.Add(companies);

                            }
                            if(!hasChanged)
                            {
                                ExistingCompany.SyncDate = DateTime.Now;
                                ExistingCompany.SyncStatus = "No new Update";
                            }

                        }
                        else
                        {
                            var AddCompanies = new Company
                            {
                                CompanyNo = companies.Company_No,
                                CompanyCode = companies.Company_Code,
                                CompanyName = companies.Company_Name,
                                CreatedAt = DateTime.Now,
                                AddedBy = command.Added_By,
                                SyncDate = DateTime.Now,
                                SyncStatus = "New Added"

                            };

                            AvailableSync.Add(companies);
                            await _context.Companies.AddAsync(AddCompanies);
                        }

                    }

                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    CompanyCodeNullOrEmpty,
                    CompanyNameNullOrEmpty,


                };


                if (DuplicateSync.Count == 0 && CompanyCodeNullOrEmpty.Count == 0 && CompanyNameNullOrEmpty.Count == 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Success(resultList);




            }
        }


    }
}
