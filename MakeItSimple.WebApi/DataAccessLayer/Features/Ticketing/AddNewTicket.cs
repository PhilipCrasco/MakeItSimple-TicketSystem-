using Google.Protobuf.WellKnownTypes;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing
{
    public class AddNewTicket
    {
        public class AddNewTicketResult
        {

            public int Id { get; set; }
            public int DepartmentId { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime Created_At { get; set; }

            public List<TicketConcerns> Concerns { get; set; }

            public class TicketConcerns
            {
                public int TicketConcernId { get; set; }
                public int SubUnitId { get; set; }

                public int ChannelId { get; set; }

                public int ChannelUserId { get; set; }

                public string Concern_Details { get; set; }

                public int CategoryId { get; set; }

                public int SubCategoryId { get; set; }

                public DateTime Start_Date { get; set; }

                public DateTime Target_Date { get; set; }

            }

        }

        public class AddNewTicketCommand : IRequest<Result>
        {
            public int  DepartmentId { get; set; }
            public Guid? Added_By { get; set; }

            public IEnumerable<TicketConcerns> Concerns { get; set; }

            public class TicketConcerns
            {
              public int  SubUnitId { get; set; } 
                
              public int  ChannelId { get; set; }

              public int  ChannelUserId {  get; set; }
              
              public string Concern_Details { get; set; }

              public int  CategoryId { get; set; }

              public int  SubCategoryId { get; set; }

             public DateTime  Start_Date { get; set; }
             public DateTime  Target_Date { get; set; }

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
                var validationErrors = new List<Result>();

                var DepartmentNotExist = await _context.Departments.Include(x => x.SubUnits)
                    .FirstOrDefaultAsync(x => x.Id == command.DepartmentId, cancellationToken);

                if (DepartmentNotExist == null)
                {
                    return Result.Failure(TicketRequestError.DepartmentNotExist());
                }

                var addTicketTransaction = new TicketTransaction
                {
                    DepartmentId = command.DepartmentId,
                    AddedBy = command.Added_By,
                };

                await _context.TicketTransactions.AddAsync(addTicketTransaction, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var concern in command.Concerns)
                {

                    switch (await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == concern.SubUnitId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.SubUnitNotExist());
                    }

                    switch (await _context.Channels.FirstOrDefaultAsync(x => x.Id == concern.ChannelId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.ChannelNotExist());
                    }

                    switch (await _context.ChannelUsers.FirstOrDefaultAsync(x => x.Id == concern.ChannelUserId, cancellationToken))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.ChannelUserNotExist());
                    }

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

                    if(string.IsNullOrEmpty(concern.Concern_Details))
                    {
                        return Result.Failure(TicketRequestError.ConcernDetailsNotNull());
                    }

                    if (concern.Start_Date > concern.Target_Date)
                    {
                        return Result.Failure(TicketRequestError.DateTimeInvalid());
                    }

                    var addConcern = new TicketConcern
                    {
                        TicketTransactionId = addTicketTransaction.Id,
                        SubUnitId = concern.SubUnitId,
                        ChannelId = concern.ChannelId,
                        ChannelUserId = concern.ChannelUserId,
                        ConcernDetails = concern.Concern_Details,
                        CategoryId = concern.CategoryId,
                        SubCategoryId = concern.SubCategoryId,
                        StartDate = concern.Start_Date,
                        TargetDate = concern.Target_Date,
                        AddedBy = addTicketTransaction.AddedBy,
                        CreatedAt = addTicketTransaction.CreatedAt,

                    };

                    await _context.TicketConcerns.AddAsync(addConcern, cancellationToken);

                    concernList.Add(addConcern);

                }

                await _context.SaveChangesAsync(cancellationToken);

                var results = new AddNewTicketResult
                {
                    Id = addTicketTransaction.Id,
                    DepartmentId = addTicketTransaction.DepartmentId,
                    Added_By = addTicketTransaction.AddedBy,
                    Created_At = addTicketTransaction.CreatedAt,
                    Concerns = concernList.Select(x => new AddNewTicketResult.TicketConcerns
                    {
                        TicketConcernId = x.Id,
                        SubUnitId = x.SubUnitId,
                        ChannelId = x.ChannelId,
                        ChannelUserId = x.ChannelUserId,
                        Concern_Details = x.ConcernDetails,
                        CategoryId = x.CategoryId,
                        SubCategoryId = x.SubCategoryId,
                        Start_Date = x.StartDate,
                        Target_Date = x.TargetDate,

                    }).ToList()
                };

                return Result.Success(results);

            }

        }




    }
}
