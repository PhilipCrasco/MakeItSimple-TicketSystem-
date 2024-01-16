using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TIcketRequest
{
    public class UpdateTicketRequest
    {

        public class UpdateTicketRequestCommand : IRequest<Result>
        {
            public int ? RequestGeneratorId { get; set; }
            public int DepartmentId { get; set; }
            public int SubUnitId { get; set; }
            public int ChannelId { get; set; }
            public Guid? UserId { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid Modified_By { get; set; }

            public ICollection<Concern> ticketConcerns { get; set; }

          
            public class Concern
            {
                public int ? TicketConcernId { get; set; }
                public string Concern_Details { get; set; }
                public int CategoryId { get; set; }
                public int SubCategoryId { get; set; }
                public DateTime Start_Date { get; set; }
                public DateTime Target_Date { get; set; }

            }  

        }

        public class Handler : IRequestHandler<UpdateTicketRequestCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context) 
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateTicketRequestCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernList = new List<TicketConcern>();
                var DateToday = DateTime.Now;

                var requestGenerator = await _context.RequestGenerators.FirstOrDefaultAsync(x => x.Id == command.RequestGeneratorId, cancellationToken);

                if (requestGenerator == null)
                {
                    return Result.Failure(TicketRequestError.TicketIdNotExist());

                }

                var requestTicketConcern = await _context.TicketConcerns.Where(x => x.RequestGeneratorId == command.RequestGeneratorId).ToListAsync();

                switch (requestTicketConcern.FirstOrDefault(x => x.DepartmentId == command.DepartmentId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.DepartmentNotExist());
                }
                switch (requestTicketConcern.FirstOrDefault(x => x.SubUnitId == command.SubUnitId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.SubUnitNotExist());
                }
                switch (requestTicketConcern.FirstOrDefault(x => x.ChannelId == command.ChannelId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.ChannelNotExist());
                }
                switch (requestTicketConcern.FirstOrDefault(x => x.UserId == command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                foreach (var concerns in command.ticketConcerns)
                {

                    switch (requestTicketConcern.FirstOrDefault(x => x.CategoryId == concerns.CategoryId))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.CategoryNotExist());
                    }
                    switch (requestTicketConcern.FirstOrDefault(x => x.CategoryId == concerns.CategoryId))
                    {
                        case null:
                            return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    }
                    switch (requestTicketConcern.FirstOrDefault(x => x.ConcernDetails == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                    && x.SubCategoryId == concerns.SubCategoryId))
                    {
                        case not null:
                            return Result.Failure(TicketRequestError.DuplicateConcern());
                    }

                    if (string.IsNullOrEmpty(concerns.Concern_Details))
                    {
                        return Result.Failure(TicketRequestError.ConcernDetailsNotNull());
                    }
                    if (concerns.Start_Date > concerns.Target_Date || concerns.Target_Date > DateToday)
                    {
                        return Result.Failure(TicketRequestError.DateTimeInvalid());
                    }

                    if (command.ticketConcerns.Count(x => x.Concern_Details == concerns.Concern_Details && concerns.CategoryId == concerns.CategoryId
                    &&  x.SubCategoryId == concerns.SubCategoryId) > 1)
                    {
                        return Result.Failure(TicketRequestError.DuplicateConcern());
                    }

                    var upsertConcern = requestTicketConcern.FirstOrDefault(x => x.Id == concerns.TicketConcernId);

                    if(upsertConcern != null)
                    {
                        bool hasChanged = false;


                        if (upsertConcern.ConcernDetails != concerns.Concern_Details)
                        {
                            upsertConcern.ConcernDetails = concerns.Concern_Details;
                            hasChanged = true;
                        }

                        if (upsertConcern.CategoryId != concerns.CategoryId)
                        {
                            upsertConcern.CategoryId = concerns.CategoryId;
                            hasChanged = true;
                        }

                        if(upsertConcern.SubCategoryId != concerns.SubCategoryId)
                        {
                            upsertConcern.SubCategoryId = concerns.SubCategoryId;
                            hasChanged = true;
                        }

                        if (upsertConcern.StartDate != concerns.Start_Date)
                        {
                            upsertConcern.StartDate = concerns.Start_Date;
                            hasChanged = true;
                        }

                        if (upsertConcern.TargetDate != concerns.Target_Date)
                        {
                            upsertConcern.TargetDate = concerns.Target_Date;
                            hasChanged = true;
                        }

                        if (hasChanged)
                        {
                            upsertConcern.ModifiedBy = command.Modified_By;
                            upsertConcern.UpdatedAt = DateTime.Now;
                            ticketConcernList.Add(upsertConcern);
                        }
                        if (!hasChanged)
                        {
                            ticketConcernList.Add(upsertConcern);
                        }
                        
                    }
                    else
                    {
                        var addTicketConcern = new TicketConcern
                        {
                            RequestGeneratorId = requestGenerator.Id,
                            DepartmentId = command.DepartmentId,
                            SubUnitId = command.SubUnitId,
                            ChannelId = command.ChannelId,
                            UserId = command.UserId,
                            ConcernDetails = concerns.Concern_Details,
                            CategoryId = concerns.CategoryId,
                            SubCategoryId = concerns.SubCategoryId,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now,

                        };

                        await _context.AddAsync(addTicketConcern, cancellationToken);
                        ticketConcernList.Add(addTicketConcern);
                    }

                }

                await _context.SaveChangesAsync(cancellationToken);


                return Result.Success("Transaction Success");

            }
        }
    }
}
