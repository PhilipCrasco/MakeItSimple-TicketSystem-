using FluentValidation;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.AddNewChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.ChannelSetup.UpdateChannel;


namespace MakeItSimple.WebApi.Models.Setup.ChannelSetup
{
    public partial class Channel
    {
        public class ChannelValidator : AbstractValidator<AddNewChannelCommand>
        {
            public ChannelValidator()
            {
                RuleFor(x => x.Channel_Name).NotEmpty().WithMessage("Sub unit code is required")
                    .MinimumLength(2).WithMessage(" code must be at least 2 character long!");
            }
        }


        public class UpdateChannelValidator : AbstractValidator<UpdateChannelCommand>
        {
            public UpdateChannelValidator()
            {
                RuleFor(x => x.Channel_Name).NotEmpty().WithMessage("Sub unit code is required")
                    .MinimumLength(2).WithMessage(" code must be at least 2 character long!");
            }
        }
    }
}
