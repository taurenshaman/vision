using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Vision.Core.Interfaces;
using System.IO;

namespace Vision.Core {
  public class ImageWorker {
    public const string DefaultApiRoot_Vision = "https://westus.api.cognitive.microsoft.com/vision/v1.0";

    const string Dir_Original = "original";
    const string Dir_Compressed = "compressed";
    const string Dir_Thumbnail = "thumbnail";

    /// <summary>
    /// 最大的图像文件大小
    /// </summary>
    public const long MaxImageFileSize = 4 * 1024 * 1024;
    /// <summary>
    /// 图像质量的下降系数: 100*0.9*0.9*...
    /// </summary>
    public const float ImageQualityDecreaseFactor = 0.9f;

    IStorage OriginalImagesStorage = null;
    IStorage CompressedImagesStorage = null;
    IStorage ThumbnailsStorage = null;

    VisionServiceClient visionService = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original_images_storage">original images. maybe has metadata. filesize maybe very big.</param>
    /// <param name="compressed_images_storage">original image is compressed. MS cognitive service supports less than 4MB file.</param>
    /// <param name="storage_thumbnails">thumbnails. </param>
    public ImageWorker(IStorage original_images_storage, IStorage compressed_images_storage, IStorage storage_thumbnails) {
      OriginalImagesStorage = original_images_storage;
      CompressedImagesStorage = compressed_images_storage;
      ThumbnailsStorage = storage_thumbnails;
    }

    public void Setup(string visionSubscriptionKey, string visionApiRoot = DefaultApiRoot_Vision) {
      visionService = new VisionServiceClient( visionSubscriptionKey, visionApiRoot );
    }

    /// <summary>
    /// compress the image, remove metadata. max size 4MB.
    /// </summary>
    /// <param name="sourceImageUrl"></param>
    /// <returns></returns>
    public async Task<string> CreateCompressed(string sourceImageUrl, string filename, long maxFileSizeInBytes = MaxImageFileSize) {
      string contentType = Tools.NetHelper.GetContentTypeOfFile( sourceImageUrl );

      System.Net.WebClient client = new System.Net.WebClient();
      var srcData = await client.DownloadDataTaskAsync( sourceImageUrl );
      // convert byte[] to jpeg
      bool cr = Tools.ImageHelper.TryGetImageFromBytes( srcData, out System.Drawing.Bitmap image );
      if (!cr)
        throw new Exception( "Cannot convert byte[] to System.Drawing.Image." );

      // compress
      System.IO.MemoryStream stream = null;
      long quality = 100L;
      while (true) {
        stream = Tools.ImageHelper.CompressBitmapToStream( image, contentType, quality ); // run at least one time to remove metadata.
        if (stream.Length < maxFileSizeInBytes)
          break;
        quality = (long)( quality * ImageQualityDecreaseFactor );
      }
      
      // save
      string url = await CompressedImagesStorage.Save( Dir_Compressed, null, filename, contentType, stream );
      return url;
    }

    /// <summary>
    /// create thumbnail. return thumbnail url.
    /// </summary>
    /// <param name="sourceImageUrl"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public async Task<string> CreateThumbnail(string sourceImageUrl, string filename, int width = 300, int height = 300, bool smartCropping = true) {
      string contentType = Tools.NetHelper.GetContentTypeOfFile( sourceImageUrl );

      var data = await visionService.GetThumbnailAsync( sourceImageUrl, width, height, smartCropping );
      // convert byte[] to jpeg
      //System.Drawing.Bitmap image = null;
      //bool cr = Tools.ImageHelper.TryGetImageFromBytes( data, out image );
      //if (!cr)
      //  throw new Exception( "Cannot convert byte[] to System.Drawing.Image." );

      // convert byte[] to stream
      MemoryStream stream = new MemoryStream( data );

      // save
      string url = await ThumbnailsStorage.Save( Dir_Thumbnail, null, filename, contentType, stream );
      return url;
    }

    public async Task<string> CpmputeImageHash(string rootDirectory, string[] leftSubDirectories, string filename) {
      string tmp = Path.Combine( leftSubDirectories );
      string filepath = Path.Combine( rootDirectory, tmp, filename );

      string pythonExePath = System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigConstants.Key_PythonExecutePath];
      string pythonScriptPath = System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigConstants.Key_PythonScriptPath_ImageHash];

      string hash = await Vision.Runtime.PythonCommond.ComputeImageWHash( pythonExePath, pythonScriptPath,filepath );
      return hash;
    }


  }

}
