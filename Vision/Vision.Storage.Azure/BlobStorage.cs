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

    const string BlobUriPrefix = "https://{0}.blob.core.windows.net/";

    public BlobStorage( string connection) {
      storageAccount = CloudStorageAccount.Parse( connection );
      blobClient = storageAccount.CreateCloudBlobClient();
    }

    async Task<CloudBlockBlob> GetCloudBlockBlob(string rootDirectory, string[] leftSubDirectories, string filename) {
      var container = blobClient.GetContainerReference( rootDirectory );
      await container.CreateIfNotExistsAsync();

      CloudBlockBlob blockBlob = null;
      if (leftSubDirectories == null || leftSubDirectories.Length == 0) {
        blockBlob = container.GetBlockBlobReference( filename );
      }
      else {
        string filepath = string.Join( "/", leftSubDirectories, filename );
        blockBlob = container.GetBlockBlobReference( filepath );
      }
      return blockBlob;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootDirectory">The root directory name.</param>
    /// <param name="leftSubDirectories">The sub directories in the root directory. will be combined if not empty. e.g. string.Join( "/", leftSubDirectories, filename )</param>
    /// <param name="stream"></param>
    /// <param name="contentType"></param>
    /// <param name="filename"></param>
    /// <returns>URI of the file.</returns>
    public async Task<string> Save(string rootDirectory, string[] leftSubDirectories, string filename, string contentType, Stream stream) {
      CloudBlockBlob blockBlob = await GetCloudBlockBlob( rootDirectory, leftSubDirectories, filename );
      blockBlob.Properties.ContentType = contentType;
      blockBlob.SetProperties();

      //await blockBlob.UploadFromStreamAsync( stream );

      var id = Convert.ToBase64String( BitConverter.GetBytes( 0 ) );
      await blockBlob.PutBlockAsync( id, stream, null );
      string[] ids = new string[] { id };
      await blockBlob.PutBlockListAsync( ids );

      return blockBlob.Uri.ToString();
    }

    public async Task<string> Rename(string rootDirectory, string[] leftSubDirectories, string oldFilename, string newFilename) {
      if(string.IsNullOrWhiteSpace(oldFilename) || string.IsNullOrWhiteSpace(newFilename)) {
        throw new ArgumentNullException( "oldFilename/newFilename" );
      }

      CloudBlockBlob oldBlob = await GetCloudBlockBlob( rootDirectory, leftSubDirectories, oldFilename );
      if (oldFilename == newFilename) {
        return oldBlob.Uri.ToString();
      }
      // copy
      CloudBlockBlob newBlob = await GetCloudBlockBlob( rootDirectory, leftSubDirectories, newFilename );
      await newBlob.StartCopyAsync( oldBlob );

      // delete old
      await oldBlob.DeleteIfExistsAsync();

      // return
      return newBlob.Uri.ToString();
    }

  }

}
