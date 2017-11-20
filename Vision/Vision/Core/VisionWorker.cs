using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using Vision.Core.Interfaces;

namespace Vision.Core {
  public class VisionWorker {
    public const string DefaultApiRoot_Emotion = "https://westus.api.cognitive.microsoft.com/emotion/v1.0";
    public const string DefaultApiRoot_Face = "https://westus.api.cognitive.microsoft.com/face/v1.0";
    public const string DefaultApiRoot_Vision = "https://westus.api.cognitive.microsoft.com/vision/v1.0";

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

    EmotionServiceClient emotionService = null;
    FaceServiceClient faceService = null;
    VisionServiceClient visionService = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original_images_storage">original images. maybe has metadata. filesize maybe very big.</param>
    /// <param name="compressed_images_storage">original image is compressed. MS cognitive service supports less than 4MB file.</param>
    /// <param name="storage_thumbnails">thumbnails. </param>
    public VisionWorker(IStorage original_images_storage, IStorage compressed_images_storage, IStorage storage_thumbnails ) {
      OriginalImagesStorage = original_images_storage;
      CompressedImagesStorage = compressed_images_storage;
      ThumbnailsStorage = storage_thumbnails;
    }

    public void Setup(string emotionSubscriptionKey,
      string faceSubscriptionKey,
      string visionSubscriptionKey,
      string emotionApiRoot = DefaultApiRoot_Emotion,
      string faceApiRoot = DefaultApiRoot_Face,
      string visionApiRoot = DefaultApiRoot_Vision) {

      emotionService = new EmotionServiceClient( emotionSubscriptionKey, emotionApiRoot );
      faceService = new FaceServiceClient( faceSubscriptionKey, faceApiRoot );
      visionService = new VisionServiceClient( visionSubscriptionKey, visionApiRoot );
    }

    /// <summary>
    /// compress the image, remove metadata. max size 4MB.
    /// </summary>
    /// <param name="sourceImageUrl"></param>
    /// <returns></returns>
    public async Task<string> CreateCompressed(string sourceImageUrl) {
      System.Net.WebClient client = new System.Net.WebClient();
      var srcData = await client.DownloadDataTaskAsync( sourceImageUrl );
      // convert byte[] to jpeg
      System.Drawing.Bitmap image = null;
      bool cr = Tools.ImageHelper.TryGetImageFromBytes( srcData, out image );
      if (!cr)
        throw new Exception( "Cannot convert byte[] to System.Drawing.Image." );
      
      // compress
      System.IO.MemoryStream stream = null;
      long quality = 100L;
      while (true) {
        stream = Tools.ImageHelper.CompressBitmapToStream( image, quality ); // run at least one time to remove metadata.
        if (stream.Length < MaxImageFileSize)
          break;
        quality = (long)( quality * ImageQualityDecreaseFactor );
      }

      // save

      // return url
      return null;
    }

    /// <summary>
    /// create thumbnail. return thumbnail url.
    /// TODO
    /// </summary>
    /// <param name="sourceImageUrl"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public async Task<string> CreateThumbnail(string sourceImageUrl, int width = 300, int height = 300) {
      var data = await visionService.GetThumbnailAsync( sourceImageUrl, width, height );
      // convert byte[] to jpeg
      System.Drawing.Bitmap image = null;
      bool cr = Tools.ImageHelper.TryGetImageFromBytes( data, out image );
      if (!cr)
        throw new Exception( "Cannot convert byte[] to System.Drawing.Image." );

      // save

      // return url
      return null;
    }

    public async Task ProcessPhoto(string imageUrl) {
      var describeResult = await visionService.DescribeAsync( imageUrl, 3 );
      //describeResult.

      var tagsResult = await visionService.GetTagsAsync( imageUrl );
    

      VisualFeature[] visualFeatures = new VisualFeature[] {
        VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags
      };
      var analysisResult = await visionService.AnalyzeImageAsync( imageUrl, visualFeatures );
      var facesRect = analysisResult.Faces.Select( i => new Rectangle() {
        Left = i.FaceRectangle.Left,
        Top = i.FaceRectangle.Top,
        Width = i.FaceRectangle.Width,
        Height = i.FaceRectangle.Height
      } )
      .ToArray();
      var emotionResult = await emotionService.RecognizeAsync( imageUrl, facesRect );
    }

    public void ProcessImage() {

    }

  }

}
