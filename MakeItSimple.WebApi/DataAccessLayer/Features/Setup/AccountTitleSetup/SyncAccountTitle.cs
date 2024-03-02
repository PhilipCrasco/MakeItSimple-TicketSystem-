using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.AccountTitleSetup
{
    public class SyncAccountTitle
    {
        public class SyncAccountTitleCommand : IRequest<Result>
        {

            public ICollection<AccountTitleResultCommand> AccountTitle { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }

            public class AccountTitleResultCommand
            {
                public int Account_No { get; set; }
                public string Account_Code { get; set; }
                public string Account_Titles { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }
            }

        }

        public class Handler : IRequestHandler<SyncAccountTitleCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncAccountTitleCommand command, CancellationToken cancellationToken)
            {
                var AllInputSync = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();
                var AvailableSync = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();
                var UpdateSync = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();
                var DuplicateSync = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();
                var AccountCodeNullOrEmpty = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();
                var AccountNameNullOrEmpty = new List<SyncAccountTitleCommand.AccountTitleResultCommand>();

                foreach (SyncAccountTitleCommand.AccountTitleResultCommand accountTitle in command.AccountTitle)
                {
                    if (string.IsNullOrEmpty(accountTitle.Account_Code))
                    {
                        AccountCodeNullOrEmpty.Add(accountTitle);
                        continue;
                    }

                    if (string.IsNullOrEmpty(accountTitle.Account_Titles))
                    {
                        AccountNameNullOrEmpty.Add(accountTitle);
                        continue;
                    }

                    if (command.AccountTitle.Count(x => x.Account_Code == accountTitle.Account_Code && x.Account_Titles == accountTitle.Account_Titles) > 1)
                    {
                        DuplicateSync.Add(accountTitle);
                    }
                    else
                    {
                        var ExistingAccountTitle = await _context.AccountTitles.FirstOrDefaultAsync(x => x.AccountNo == accountTitle.Account_No, cancellationToken);
                        if (ExistingAccountTitle != null)
                        {
                            bool hasChanged = false;

                            if (ExistingAccountTitle.AccountCode != accountTitle.Account_Code)
                            {
                                ExistingAccountTitle.AccountCode = accountTitle.Account_Code;
                                hasChanged = true;
                            }

                            if (ExistingAccountTitle.AccountTitles != accountTitle.Account_Titles)
                            {
                                ExistingAccountTitle.AccountTitles = accountTitle.Account_Titles;
                                hasChanged = true;
                            }

                            if (hasChanged)
                            {
                                ExistingAccountTitle.UpdatedAt = DateTime.Now;
                                ExistingAccountTitle.ModifiedBy = command.Modified_By;
                                ExistingAccountTitle.SyncDate = DateTime.Now;
                                ExistingAccountTitle.SyncStatus = "New Update";

                                UpdateSync.Add(accountTitle);

                            }
                            if (!hasChanged)
                            {
                                ExistingAccountTitle.SyncDate = DateTime.Now;
                                ExistingAccountTitle.SyncStatus = "No new Update";
                            }

                        }
                        else
                        {
                            var AddAccountTitle = new AccountTitle
                            {
                                AccountNo = accountTitle.Account_No,
                                AccountCode = accountTitle.Account_Code,
                                AccountTitles = accountTitle.Account_Titles,
                                CreatedAt = DateTime.Now,
                                AddedBy = command.Added_By,
                                SyncDate = DateTime.Now,
                                SyncStatus = "New Added"

                            };

                            AvailableSync.Add(accountTitle);
                            await _context.AccountTitles.AddAsync(AddAccountTitle);
                        }

                    }
                    AllInputSync.Add(accountTitle);
                }

                var allInputByNo = AllInputSync.Select(x => x.Account_No);

                var allDataInput = await _context.AccountTitles.Where(x => !allInputByNo.Contains(x.AccountNo)).ToListAsync();

                foreach (var account in allDataInput)
                {
                    _context.Remove(account);
                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    AccountCodeNullOrEmpty,
                    AccountNameNullOrEmpty,


                };


                if (DuplicateSync.Count == 0 && AccountCodeNullOrEmpty.Count == 0 && AccountNameNullOrEmpty.Count == 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Warning(resultList);
            }
        }


    }

}
