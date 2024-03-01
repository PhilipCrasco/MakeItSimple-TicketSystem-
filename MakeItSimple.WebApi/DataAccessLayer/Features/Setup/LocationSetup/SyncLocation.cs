﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Features.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.CompanySetup.SyncCompany;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup.SyncSubUnit;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.LocationSetup
{
    public class SyncLocation
    {
        public class SyncLocationCommand : IRequest<Result>
        {
            public ICollection<LocationResultCommand> locations { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public class LocationResultCommand
            {

                public int Location_No { get; set; }
                public string Location_Code { get; set; }
                public string Location_Name { get; set; }
                //public string SubUnit_Name { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Update_dAt { get; set; }

                public List<UpsertSubunitList> UpsertSubunitLists {  get; set; }

                public class UpsertSubunitList
                {
                    public string SubUnit_Name { get; set;}
                }

            }

        }


        public class Handler : IRequestHandler<SyncLocationCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncLocationCommand command, CancellationToken cancellationToken)
            {
                var AllInputSync = new List<SyncLocationCommand.LocationResultCommand>();
                var AvailableSync = new List<SyncLocationCommand.LocationResultCommand>();
                var UpdateSync = new List<SyncLocationCommand.LocationResultCommand>();
                var DuplicateSync = new List<SyncLocationCommand.LocationResultCommand>();
                var LocationCodeNullOrEmpty = new List<SyncLocationCommand.LocationResultCommand>();
                var LocationNameNullOrEmpty = new List<SyncLocationCommand.LocationResultCommand>();
                var SubUnitNotExist = new List<SyncLocationCommand.LocationResultCommand>();

                foreach (var location in command.locations)
                {

                    foreach(var subUnit in location.UpsertSubunitLists)
                    {
                        var subUnitNotExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitName == subUnit.SubUnit_Name, cancellationToken);
                        if (subUnitNotExist == null)
                        {
                            SubUnitNotExist.Add(location);
                            continue;
                        }

                        if (string.IsNullOrEmpty(location.Location_Code))
                        {
                            LocationCodeNullOrEmpty.Add(location);
                            continue;
                        }

                        if (string.IsNullOrEmpty(location.Location_Name))
                        {
                            LocationNameNullOrEmpty.Add(location);
                            continue;
                        }

                        if (command.locations.Count(x => x.Location_Code == location.Location_Code && x.Location_Name == location.Location_Name) > 1)
                        {
                            DuplicateSync.Add(location);

                        }
                        else
                        {
                            var ExistingLocation = await _context.Locations.FirstOrDefaultAsync(x => x.LocationNo == location.Location_No, cancellationToken);

                            if (ExistingLocation != null)
                            {
                                bool hasChanged = false;

                                if (ExistingLocation.SubUnitId != subUnitNotExist.Id)
                                {
                                    ExistingLocation.SubUnitId = subUnitNotExist.Id;
                                    hasChanged = true;
                                }

                                if (ExistingLocation.LocationCode != location.Location_Code)
                                {
                                    ExistingLocation.LocationCode = location.Location_Code;
                                    hasChanged = true;
                                }

                                if (ExistingLocation.LocationName != location.Location_Name)
                                {
                                    ExistingLocation.LocationName = location.Location_Name;
                                    hasChanged = true;
                                }

                                if (hasChanged)
                                {
                                    ExistingLocation.UpdatedAt = DateTime.Now;
                                    ExistingLocation.ModifiedBy = command.Modified_By;
                                    ExistingLocation.SyncDate = DateTime.Now;
                                    ExistingLocation.SyncStatus = "New Update";

                                    UpdateSync.Add(location);

                                }
                                if (!hasChanged)
                                {
                                    ExistingLocation.SyncDate = DateTime.Now;
                                    ExistingLocation.SyncStatus = "No new Update";
                                }

                            }
                            else
                            {
                                var AddLocation = new Location
                                {
                                    LocationNo = location.Location_No,
                                    LocationCode = location.Location_Code,
                                    LocationName = location.Location_Name,
                                    SubUnitId = subUnitNotExist.Id,
                                    CreatedAt = DateTime.Now,
                                    AddedBy = command.Added_By,
                                    SyncDate = DateTime.Now,
                                    SyncStatus = "New Added"

                                };

                                AvailableSync.Add(location);
                                await _context.Locations.AddAsync(AddLocation);
                            }
                        }
                    }

                    AllInputSync.Add(location);
                }

                var allInputByNo = AllInputSync.Select(x => x.Location_No);

                var allDataInput = await _context.Locations.Where(x => !allInputByNo.Contains(x.LocationNo)).ToListAsync();

                foreach (var location in allDataInput)
                {
                    //_context.Remove(department);
                    location.IsActive = false;
                    
                    var subUnitlist = await _context.SubUnits.Where(x => x.Id == location.SubUnitId).ToListAsync();

                    foreach(var subUnit in subUnitlist)
                    {
                        subUnit.IsActive = false;

                        var channelList = await _context.Channels.Where(x => x.SubUnitId == subUnit.Id).ToListAsync();

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
                    }

                }

                var resultList = new
                {
                    AvailableSync,
                    UpdateSync,
                    DuplicateSync,
                    LocationCodeNullOrEmpty,
                    LocationNameNullOrEmpty,
                    SubUnitNotExist


                };

                if (DuplicateSync.Count == 0 && LocationCodeNullOrEmpty.Count == 0 && LocationNameNullOrEmpty.Count == 0 && SubUnitNotExist.Count == 0)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success("Successfully sync data");
                }

                return Result.Success(resultList);

            }
        }




    }
}
