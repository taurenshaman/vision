using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Core.Interfaces {
  public interface IStorage {
    Task<string> Save(string rootDirectory, string[] leftSubDirectories, string filename, string contentType, Stream stream);

  }

}
