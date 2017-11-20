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
