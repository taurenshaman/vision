using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json.Linq;

namespace Vision.Tools {
  public static class JsonHelper {
    public static JObject ConvertToJson(Rectangle source) {
      JObject rect = new JObject();
      rect["left"] = source.Left;
      rect["top"] = source.Top;
      rect["width"] = source.Width;
      rect["height"] = source.Height;
      return rect;
    }
    public static JObject ConvertToJson(Microsoft.ProjectOxford.Face.Contract.FaceRectangle source) {
      JObject rect = new JObject();
      rect["left"] = source.Left;
      rect["top"] = source.Top;
      rect["width"] = source.Width;
      rect["height"] = source.Height;
      return rect;
    }

    public static JObject ConvertToJson( EmotionScores source ) {
      JObject result = new JObject();
      result["anger"] = source.Anger;
      result["contempt"] = source.Contempt;
      result["disgust"] = source.Disgust;
      result["fear"] = source.Fear;
      result["happiness"] = source.Happiness;
      result["neutral"] = source.Neutral;
      result["sadness"] = source.Sadness;
      result["surprise"] = source.Surprise;
      return result;
    }

    public static JObject ConvertToJson(Microsoft.ProjectOxford.Vision.Contract.Tag source) {
      JObject rect = new JObject();
      rect["name"] = source.Name;
      rect["hint"] = source.Hint;
      rect["confidence"] = source.Confidence;
      return rect;
    }
    // TODO
    public static JObject ConvertToJson(Microsoft.ProjectOxford.Face.Contract.FaceLandmarks source) {
      JObject result = new JObject();

      return result;
    }

    public static JObject ConvertToJson(Microsoft.ProjectOxford.Face.Contract.Face face) {
      JObject result = new JObject();
      result["id"] = face.FaceId.ToString().ToLowerInvariant();
      result["reactangle"] = ConvertToJson( face.FaceRectangle );
      result["landmarks"] = ConvertToJson( face.FaceLandmarks );
      return result;
    }

    //public static JObject ConvertToJson( EmotionScores scores, Rectangle rect) {
    //  JObject emotion = new JObject();
    //  emotion["faceRectangle"] = ConvertToJson( rect );
    //  emotion["scores"] = ConvertToJson( scores );
    //  return emotion;
    //}

    public static void ConvertToJson(Microsoft.ProjectOxford.Emotion.Contract.Emotion emotion, Microsoft.ProjectOxford.Face.Contract.Face face ) {
      
    }

  }

}
