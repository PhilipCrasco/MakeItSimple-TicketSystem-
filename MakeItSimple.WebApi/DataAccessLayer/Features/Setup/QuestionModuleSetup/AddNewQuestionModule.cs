using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.QuestionModuleSetup
{
    public class AddNewQuestionModule
    {


        public class AddNewQuestionModuleCommand : IRequest<Result>
        {

            public int ? Id { get; set; }


            public class AddQuestionModuleForm
            {
                public int? Id { set; get; }
                public int ? FormId { get; set; }
                public int ? QuestionModuleId { set; get; }

               
            }

        }

        public class Handler : IRequestHandler<AddNewQuestionModuleCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewQuestionModuleCommand command, CancellationToken cancellationToken)
            {

                var questionExist = await _context.QuestionModules
                    .FirstOrDefaultAsync(x => x.Id == command.Id);

                if (questionExist is not null)
                {
                    


                }
                else
                {

                }


                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
