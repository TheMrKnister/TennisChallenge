using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;

namespace TennisWeb.Utils
{
  public class Picture
  {
    public const string UploadImagePath = "~/uplimg/";
    public const string AdvertisementImagePath = "~/advertisement_images/";

    public static string GeneratePictureName(HttpPostedFileBase file)
    {
      string filePath = String.Empty;
      if (file != null && file.ContentLength > 0)
      {
        filePath = Guid.NewGuid().ToString();
        switch (file.ContentType)
        {
          case "image/jpeg":
            filePath += ".jpg";
            break;
          case "image/png":
            filePath += ".png";
            break;
          case "image/gif":
            filePath += ".gif";
            break;
          default:
            filePath = String.Empty;
            break;
        }
      }
      return filePath;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x position of the rectangle to cut out of the picture</param>
    /// <param name="y">y position of the rectangle to cut out of the picture</param>
    /// <param name="width">width position of the rectangle to cut out of the picture</param>
    /// <param name="height">height position of the rectangle to cut out of the picture</param>
    /// <param name="picture">input picture stream</param>
    /// <param name="newPicture">output picture stream</param>
    public static void ConvertPicture(float x, float y, float width, float height, Stream picture, Stream newPicture)
    {
      using (var image = Image.FromStream(picture))
      using (var newBitmap = new Bitmap(160, 160))
      using (var newGraph = Graphics.FromImage(newBitmap))
      {
        if (width <= 0 || height <= 0)
        {
          x = 0;
          y = 0;
          width = image.Width;
          height = image.Height;
        }
        newGraph.CompositingQuality = CompositingQuality.HighQuality;
        newGraph.SmoothingMode = SmoothingMode.HighQuality;
        newGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

        var imageRectangle = new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
        newGraph.DrawImage(image, new Rectangle(0, 0, 160, 160), imageRectangle, GraphicsUnit.Pixel);

        newBitmap.Save(newPicture, image.RawFormat);
      }
    }

    public static void ProcessPicture(string fullFileName, float crop_x, float crop_y, float crop_w, float crop_h)
    {
      if (Path.IsPathRooted(fullFileName))
      {
        var oldPicture = new MemoryStream();
        var picture = new FileStream(fullFileName, FileMode.Open);
        picture.CopyTo(oldPicture);
        picture.Close();
        var newPicture = new FileStream(fullFileName, FileMode.Create);

        ConvertPicture(crop_x, crop_y, crop_w, crop_h, oldPicture, newPicture);
        oldPicture.Close();
        newPicture.Close();
      }
    }
  }
}