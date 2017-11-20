using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Core.Interfaces {
  public interface IStorage {
    Task<string> Save(string containerName, Stream stream, string contentType, string filename);

  }

}
