using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.SubUnitSetup
{
    public class UpsertSubUnit
    {
        public class UpsertSubUnitCommand : IRequest<Result>
        {
            public int? SubUnitId { get; set; }
            public int ? UnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public Guid ? Added_By { get; set; }
            public Guid ? Modified_By { get; set; }

            public List<Location> Locations { get; set; }

            public class Location
            {

               public string  Location_Code { get; set; }
               public string Location_Name { get; set; }
            }

        }


        public class Handler : IRequestHandler<UpsertSubUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertSubUnitCommand command, CancellationToken cancellationToken)
            {
                var unitNotExist = await _context.Units.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);
                if (unitNotExist == null)
                {
                    return Result.Failure(SubUnitError.SubUnitNotExist());
                }


                var subUnitExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.Id == command.SubUnitId, cancellationToken);

                if (subUnitExist != null)
                {
                    var subUnitCodeAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitCode == command.SubUnit_Code
                    && subUnitExist.SubUnitCode != command.SubUnit_Code, cancellationToken);

                    if (subUnitCodeAlreadyExist != null)
                    {
                        return Result.Failure(SubUnitError.SubUnitCodeAlreadyExist(command.SubUnit_Code));
                    }

                    var subUnitNameAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitName == command.SubUnit_Name
                    && subUnitExist.SubUnitName != command.SubUnit_Name, cancellationToken);

                    if (subUnitCodeAlreadyExist != null)
                    {
                        return Result.Failure(SubUnitError.SubUnitNameAlreadyExist(command.SubUnit_Name));
                    }

                    bool hasChange = false;

                    if(subUnitExist.UnitId != command.UnitId)
                    {
                        subUnitExist.UnitId = command.UnitId;
                        hasChange = true;
                    }

                    if(subUnitExist.SubUnitCode != command.SubUnit_Code)
                    {
                        subUnitExist.SubUnitCode = command.SubUnit_Code;
                        hasChange = true;
                    }

                    if(subUnitExist.SubUnitName != command.SubUnit_Name)
                    {
                        subUnitExist.SubUnitName = command.SubUnit_Name;
                        hasChange = true;
                    }

                    if(hasChange)
                    {
                        subUnitExist.ModifiedBy = command.Modified_By;
                        subUnitExist.UpdatedAt = DateTime.Now;
                    }



                }
                else
                {

                    var subUnitCodeAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitCode == command.SubUnit_Code, cancellationToken);

                    if (subUnitCodeAlreadyExist != null)
                    {
                        return Result.Failure(SubUnitError.SubUnitCodeAlreadyExist(command.SubUnit_Code));
                    }

                    var subUnitNameAlreadyExist = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitName == command.SubUnit_Name, cancellationToken);

                    if (subUnitCodeAlreadyExist != null)
                    {
                        return Result.Failure(SubUnitError.SubUnitNameAlreadyExist(command.SubUnit_Name));
                    }

                    var addSubUnit = new SubUnit
                    {
                        UnitId = command.UnitId,
                        SubUnitCode = command.SubUnit_Code,
                        SubUnitName = command.SubUnit_Name,
                        AddedBy = command.Added_By,
                    };

                    await _context.SubUnits.AddAsync(addSubUnit);
                    await _context.SaveChangesAsync(cancellationToken);  
                    
                    foreach(var location in command.Locations)
                    {
                        var locationExist = await _context.Locations.FirstOrDefaultAsync(x => x.LocationCode == location.Location_Code, cancellationToken);
                        if (locationExist == null)
                        {
                            return Result.Failure(SubUnitError.LocationNotExist());
                        }

                        var addLocation = new Location
                        {
                            LocationCode = locationExist.LocationCode,
                            LocationName = locationExist.LocationName,
                            SubUnitId = addSubUnit.Id,
                            AddedBy = command.Added_By
                        };

                        await _context.Locations.AddAsync(addLocation, cancellationToken);

                    }

                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
