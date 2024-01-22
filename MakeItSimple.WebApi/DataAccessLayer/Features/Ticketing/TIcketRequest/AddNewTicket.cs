using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class AddNewTicket
    {
        public class AddNewTicketResult
        {

            public int? RequestGeneratorId { get; set; }

            public List<TicketConcerns> Concerns { get; set; }

            public class TicketConcerns
            {
                public int TicketConcernId { get; set; }
                public int DepartmentId { get; set; }
                public int SubUnitId { get; set; }

                public int ChannelId { get; set; }

                public Guid? UserId { get; set; }

                public string Concern_Details { get; set; }

                public int CategoryId { get; set; }

                public int SubCategoryId { get; set; }

                public DateTime Start_Date { get; set; }

                public DateTime Target_Date { get; set; }

                public Guid? Added_By { get; set; }
                public DateTime Created_At { get; set; }

            }

        }

        public class AddNewTicketCommand : IRequest<Result>
        {

            public int DepartmentId { get; set; }
            public int SubUnitId { get; set; }
            public int ChannelId { get; set; }
            public Guid? UserId { get; set; }
            public Guid? Added_By { get; set; }
            public ICollection<TicketConcerns> Concerns { get; set; }

            public class TicketConcerns
            {

                public string Concern_Details { get; set; }
                public int CategoryId { get; set; }
                public int SubCategoryId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set; }

            }

        }

        public class Handler : IRequestHandler<AddNewTicketCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddNewTicketCommand command, CancellationToken cancellationToken)
            {

                var concernList = new List<TicketConcern>();
                var DateToday = DateTime.Now;

                var addRequestGenerator = new RequestGenerator { };


                await _context.RequestGenerators.AddAsync(addRequestGenerator, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);


                switch (await _context.Departments.FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TicketRequestError.DepartmentNotExist());
                }
                switch (await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TicketRequestError.SubUnitNotExist());
                }

                switch (await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                switch (await _context.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }


                foreach (var concern in command.Concerns)
                {

                    switch (await _context.Categories.FirstOrDefaultAsync(x => x.Id == concern.CategoryId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.CategoryNotExist());
                    }

                    switch (await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == concern.SubCategoryId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    }

                    if (string.IsNullOrEmpty(concern.Concern_Details))
                    {
                        return Result.Failure(TicketRequestError.ConcernDetailsNotNull());
                    }

                    if (concern.Start_Date > concern.Target_Date || concern.Target_Date < DateToday)
                    {
                        return Result.Failure(TicketRequestError.DateTimeInvalid());
                    }

                    var addConcern = new TicketConcern
                    {
                        RequestGeneratorId = addRequestGenerator.Id,
                        DepartmentId = command.DepartmentId,
                        SubUnitId = command.SubUnitId,
                        ChannelId = command.ChannelId,
                        UserId = command.UserId,
                        ConcernDetails = concern.Concern_Details,
                        CategoryId = concern.CategoryId,
                        SubCategoryId = concern.SubCategoryId,
                        StartDate = concern.Start_Date,
                        TargetDate = concern.Target_Date,
                        AddedBy = command.Added_By,
                        CreatedAt = DateTime.Now,

                    };

                    await _context.TicketConcerns.AddAsync(addConcern, cancellationToken);

                    concernList.Add(addConcern);

                }

                await _context.SaveChangesAsync(cancellationToken);

                var results = new AddNewTicketResult
                {
                    RequestGeneratorId = addRequestGenerator.Id,

                    Concerns = concernList.Select(x => new AddNewTicketResult.TicketConcerns
                    {
                        TicketConcernId = x.Id,
                        DepartmentId = x.DepartmentId,
                        SubUnitId = x.SubUnitId,
                        ChannelId = x.ChannelId,
                        UserId = x.UserId,
                        Concern_Details = x.ConcernDetails,
                        CategoryId = x.CategoryId,
                        SubCategoryId = x.SubCategoryId,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate,
                        Added_By = x.AddedBy,
                        Created_At = x.CreatedAt,

                    }).ToList()
                };

                return Result.Success(results);

            }

        }




    }
}
