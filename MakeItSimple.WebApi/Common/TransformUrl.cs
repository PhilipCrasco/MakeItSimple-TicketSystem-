
using CloudinaryDotNet;
using MakeItSimple.WebApi.Common.Cloudinary;
using MakeItSimple.WebApi.DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace MakeItSimple.WebApi.Common
{
    public class TransformUrl
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public TransformUrl(IOptions<CloudinaryOption> options)
        {
 
            var account = new Account(
                options.Value.Cloudname,
                options.Value.ApiKey,
                options.Value.ApiSecret
                );
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }


        public string TransformUrlForViewOnly(string url, string fileName)
        {

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (extension == ".pdf")
            {
     
                return $"https://drive.google.com/viewerng/viewer?embedded=true&url={url}";
            }
            else if (extension == ".docx" || extension == ".xlsx")
            {
                return $"https://view.officeapps.live.com/op/view.aspx?src={url}";
            }
            return url;

        }






    }
}
