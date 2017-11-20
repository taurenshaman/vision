using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Tools {
  public class NetHelper {
    public static string GetContentTypeOfFile(string url) {
      if (string.IsNullOrWhiteSpace( url ))
        return null;
      string tmpUrl = url.ToLower();
      // images
      if (tmpUrl.EndsWith( ".jpg" ) || tmpUrl.EndsWith( "jpeg" ))
        return Constants.ContentTypes.Jpeg;
      else if (tmpUrl.EndsWith( ".png" ))
        return Constants.ContentTypes.Png;

      // semantic web
      else if (tmpUrl.EndsWith( ".rdf" ) || tmpUrl.EndsWith( ".rdfs" ))
        return Constants.ContentTypes.Rdf;
      else if (tmpUrl.EndsWith( ".owl" ) )
        return Constants.ContentTypes.Owl;
      else if (tmpUrl.EndsWith( ".ttl" ) || tmpUrl.EndsWith( ".turtle" ))
        return Constants.ContentTypes.Turtle;
      else if (tmpUrl.EndsWith( ".xml" ))
        return Constants.ContentTypes.Xml;

      return null;
    }


  }

}
