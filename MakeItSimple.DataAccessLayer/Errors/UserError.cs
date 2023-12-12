using MakeItSimple.Utility.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MakeItSimple.DataAccessLayer.Errors
{
    public class UserError
    {
      public static Error NoDataFound() =>
      new Error("User.NoDataFound", "No Data found");


    }
}
