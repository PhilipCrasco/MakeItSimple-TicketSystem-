using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeItSimple.Utility.Common
{
    public interface BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }



}
