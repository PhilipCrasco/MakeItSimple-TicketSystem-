using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.ProjectSetup;
using MediatR;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ProjectSetup
{
    public class UpsertProject
    {
        public class UpsertProjectCommand : IRequest<Result>
        {
            public int ProjectId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            public string Project_Name { get; set; }
        }

        public class Handler : IRequestHandler<UpsertProjectCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertProjectCommand command, CancellationToken cancellationToken)
            {

                var projectNameExist = await _context.Projects.FirstOrDefaultAsync(x => x.ProjectName == command.Project_Name, cancellationToken);

                if (projectNameExist != null)
                {
                    return Result.Failure(ChannelError.ProjectNameAlreadyExist(command.Project_Name));
                }

                var projectExist = await _context.Projects.FirstOrDefaultAsync(x => x.Id == command.ProjectId, cancellationToken);
                if (projectExist != null)
                {
                    bool hasChange = false; 

                    if(projectExist.ProjectName != command.Project_Name)
                    {
                        projectExist.ProjectName = command.Project_Name;
                        hasChange = true;
                    }

                    if(hasChange)
                    {
                        projectExist.ModifiedBy = command.Modified_By;
                        projectExist.UpdatedAt = DateTime.Now;
                    }

                }
                else
                {

                    var addProject = new Project
                    {
                        ProjectName = command.Project_Name,
                        AddedBy = command.Added_By,

                    };

                    await _context.Projects.AddAsync(addProject);   
                }

                await _context.SaveChangesAsync(cancellationToken);  
                return Result.Success();
            }
        }
    }
}
