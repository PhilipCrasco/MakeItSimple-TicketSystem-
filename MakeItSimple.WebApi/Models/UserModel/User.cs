using MakeItSimple.DataAccessLayer.Features.UserFeatures;
using MakeItSimple.WebApi.Common;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public Guid Id { get; set; }
        public bool IsActive { get ; set; } = true;
 
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;}



    }
}
