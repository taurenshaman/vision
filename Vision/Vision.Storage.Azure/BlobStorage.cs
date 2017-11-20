using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Vision.Core.Interfaces;

namespace Vision.Storage.Azure {
  public class BlobStorage : IStorage {
    CloudStorageAccount storageAccount = null;
    CloudBlobClient blobClient = null;

    public BlobStorage( string connection) {
      storageAccount = CloudStorageAccount.Parse( connection );
      blobClient = storageAccount.CreateCloudBlobClient();
    }

    public async Task<string> Save(string containerName, Stream stream, string contentType, string filename) {
      var container = blobClient.GetContainerReference( containerName );
      await container.CreateIfNotExistsAsync();

      CloudBlockBlob blockBlob = container.GetBlockBlobReference( filename );
      blockBlob.Properties.ContentType = contentType;
      blockBlob.SetProperties();

      await blockBlob.UploadFromStreamAsync( stream );
      return null;
    }

  }

}
