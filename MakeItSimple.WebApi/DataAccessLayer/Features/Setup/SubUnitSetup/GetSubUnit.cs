﻿using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.TeamSetup
{
    public class GetSubUnit
    {

        public class GetSubUnitResult
        {
            public int Id { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            
            public string Deparment_Name {  get; set; }

            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public int No_Of_Channels { get; set; }

            public List<User> users { get; set; }

            public class User
            {
                public Guid ? UserId { get; set; }
                public string Fullname { get; set; }
            }

            public List<Channel> channels{ get; set; }

            public class Channel
            {
                public int ChannelId { get; set; }
                public string Channel_Name { get; set; }
            } 

        }

        public class GetSubUnitQuery : UserParams, IRequest<PagedList<GetSubUnitResult>>
        {
            public string Search { get; set; }

            public bool ? Status { get; set; }

        }

        public class Handler : IRequestHandler<GetSubUnitQuery, PagedList<GetSubUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetSubUnitResult>> Handle(GetSubUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<SubUnit> subUnitQuery = _context.SubUnits.Include(x => x.Channels).Include(x => x.AddedByUser).Include(x => x.ModifiedByUser);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    subUnitQuery = subUnitQuery.Where(x => x.SubUnitCode.Contains(request.Search)
                    || x.SubUnitName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    subUnitQuery = subUnitQuery.Where(x => x.IsActive == request.Status);
                }

                var result = subUnitQuery.Select(x => new GetSubUnitResult
                {
                    Id = x.Id,
                    SubUnit_Code = x.SubUnitCode,
                    SubUnit_Name = x.SubUnitName,
                    Deparment_Name = x.Department.DepartmentName,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Updated_At = x.UpdatedAt,
                    No_Of_Channels = x.Channels.Count(),
                    users = x.Users.Where(x => x.IsActive == true).Select(x => new GetSubUnitResult.User
                    {
                        UserId = x.Id,
                        Fullname = x.Fullname,

                    }).ToList(),

                    channels = x.Channels.Where(x => x.IsActive == true).Select(x => new GetSubUnitResult.Channel
                    {
                        ChannelId = x.Id,
                        Channel_Name = x.ChannelName

                    }).ToList(),

                });

                return await PagedList<GetSubUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);

            }
        }
    }
}
