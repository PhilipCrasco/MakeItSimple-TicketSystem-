using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.UserManagement.UserAccount
{
    public class UpdateProfilePic
    {
        public sealed class UpdateProfilePicCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public IFormFile Profile_Pic { get; set; }
        }


        public class Handler : IRequestHandler<UpdateProfilePicCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly Cloudinary _cloudinary;
            private readonly TransformUrl _url;

            public Handler(MisDbContext context, IOptions<CloudinaryOption> options, TransformUrl url)
            {
                _context = context;
                var account = new Account(
                    options.Value.Cloudname,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                    );
                _cloudinary = new Cloudinary(account);
                _url = url;
            }

            public async Task<Result> Handle(UpdateProfilePicCommand command, CancellationToken cancellationToken)
            {

                var userExist = await _context.Users
                 .FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

                if (userExist == null)
                {
                    return Result.Failure(UserError.UserNotExist());
                }

                if (command.Profile_Pic is null)
                {
                    return Result.Failure(UserError.ProfilePicNull());
                }

                if (command.Profile_Pic == null || command.Profile_Pic.Length == 0)
                {
                    return Result.Failure(TicketRequestError.AttachmentNotNull());
                }

                if (command.Profile_Pic.Length > 10 * 1024 * 1024)
                {
                    return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                }

                var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png" };
                var extension = Path.GetExtension(command.Profile_Pic.FileName)?.ToLowerInvariant();

                if (extension == null || !allowedFileTypes.Contains(extension))
                {
                    return Result.Failure(TicketRequestError.InvalidAttachmentType());
                }

                await using var stream = command.Profile_Pic.OpenReadStream();

                var attachmentsParams = new ImageUploadParams
                {
                    File = new FileDescription(command.Profile_Pic.FileName, stream),
                    PublicId = $"MakeITSimple/ProfilePic/{userExist.Fullname}/{command.Profile_Pic.FileName}"
                };

                var attachmentResult = await _cloudinary.UploadAsync(attachmentsParams);
                string attachmentUrl = attachmentResult.SecureUrl.ToString();
                string transformedUrl = _url.TransformUrlForViewOnly(attachmentUrl, command.Profile_Pic.FileName);

                userExist.ProfilePic = attachmentResult.SecureUrl.ToString();
                userExist.ModifiedBy = command.UserId;
                userExist.FileName = command.Profile_Pic.FileName;
                userExist.FileSize = command.Profile_Pic.Length;
                userExist.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
