using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.Models.Setup.ChannelUserSetup
{
    public class ChannelUser 
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid ? UserId { get; set; }
        public virtual User User { get; set; }
        public int ChannelId { get; set; }  
        public virtual Channel Channel { get; set; }

        


    }
}
