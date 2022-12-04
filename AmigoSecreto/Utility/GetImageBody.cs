using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AmigoSecreto.Utility
{
    public static class GetImageBody
    {

        public static AlternateView GetEmbeddedImage(Image img)
        {
            var imageString = ImageToBase64(img);
            LinkedResource res = new LinkedResource(imageString);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public static string ImageToBase64(Image img)
        {
            using MemoryStream m = new();
            img.Save(m, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = m.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}
