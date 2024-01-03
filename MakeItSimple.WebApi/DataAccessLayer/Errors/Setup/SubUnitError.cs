using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class SubUnitError
    {
      public static Error SubUnitCodeAlreadyExist(string SubUnitCode) =>
      new Error("SubUnit.SubUnitCodeAlreadyExist", $"Sub unit code {SubUnitCode} already exist!");

      public static Error SubUnitNameAlreadyExist(string SubUnitName) =>
      new Error("SubUnit.SubUnitCodeAlreadyExist", $"Sub unit name {SubUnitName} already exist!");

      public static Error DepartmentNotExist() =>
      new Error("SubUnit.DepartmentNotExist", "Department doesn't exist!");

      public static Error SubUnitNotExist() =>
      new Error("SubUnit.SubUnitNotExist", "Sub unit doesn't exist!");
      public static Error SubUnitIsUse(string subUnit) =>
      new Error("SubUnit.SubUnitIsUse", $"Sub unit {subUnit} is use!");

      public static Error SubUnitNochanges() =>
      new Error("SubUnit.SubUnitNochanges", "No changes has made!");

    }
}
