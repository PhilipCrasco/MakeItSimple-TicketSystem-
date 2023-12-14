using MakeItSimple.WebApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeItSimple.WebApi.Common
{
    public interface BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive  { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AddedByUser")]
        public Guid ? AddedBy { get; set; }
        public User AddedByUser { get; set; }


        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }




    }
}
