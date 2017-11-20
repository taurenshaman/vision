using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace ConsoleTest {
  class Program {
    static void Main(string[] args) {
      //testImageHashUsingCommand();


      Console.WriteLine( "done." );
      Console.ReadKey();
    }

    // not work. result is same with original files: file size, metadata
    static void testCloneBitmap() {
      string sourceFile1 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170226_11_16_32_Rich.jpg"; // 5.95MB,6097KB
      string sourceFile2 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170321_08_08_23_Rich.jpg"; // 6.90MB,7068KB

      var imgSource = Bitmap.FromFile( sourceFile1 );
      var imgResult = (Bitmap)imgSource.Clone();
      imgResult.Save( @"C:\Users\jerin\Desktop\WP_20170226_11_16_32_Rich-clone.jpg" );
      imgSource.Dispose();
      imgResult.Dispose();

      imgSource = Bitmap.FromFile( sourceFile2 );
      imgResult = (Bitmap)imgSource.Clone();
      imgResult.Save( @"C:\Users\jerin\Desktop\WP_20170321_08_08_23_Rich-clone.jpg" );
      imgSource.Dispose();
      imgResult.Dispose();
    }

    // 成功移除Exif；但是对文件大小降低的很小
    static void testJpegPatcher() {
      string sourceFile1 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170226_11_16_32_Rich.jpg"; // 5.95MB,6097KB
      string sourceFile2 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170321_08_08_23_Rich.jpg"; // 6.90MB,7068KB

      var file1Data = File.ReadAllBytes( sourceFile1 );
      using (MemoryStream ms = new MemoryStream( file1Data )) {
        using (MemoryStream sOut = new MemoryStream()) {
          Vision.Image.JpegPatcher.PatchAwayExif( ms, sOut );
          var img = Image.FromStream( sOut );
          img.Save( @"C:\Users\jerin\Desktop\WP_20170226_11_16_32_Rich-clone.jpg" );
        }
      }
      // 结果：成功移除Exif。文件大小：5.89MB

      var file2Data = File.ReadAllBytes( sourceFile2 );
      using (MemoryStream ms = new MemoryStream( file2Data )) {
        using (MemoryStream sOut = new MemoryStream()) {
          Vision.Image.JpegPatcher.PatchAwayExif( ms, sOut );
          var img = Image.FromStream( sOut );
          img.Save( @"C:\Users\jerin\Desktop\WP_20170321_08_08_23_Rich-clone.jpg" );
        }
      }
      // 结果：成功移除Exif。文件大小：6.84MB
    }

    // 压缩率不好控制
    static void testEncoderParameters() {
      string sourceFile1 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170226_11_16_32_Rich.jpg"; // 5.95MB,6097KB
      string sourceFile2 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170321_08_08_23_Rich.jpg"; // 6.90MB,7068KB

      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      ImageCodecInfo ici = null;

      foreach (ImageCodecInfo codec in codecs) {
        if (codec.MimeType == "image/jpeg")
          ici = codec;
      }

      EncoderParameters ep = new EncoderParameters();
      ep.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, (long)80 );

      Bitmap bmp1 = (Bitmap)Image.FromFile( sourceFile1 );
      bmp1.Save( @"C:\Users\jerin\Desktop\WP_20170226_11_16_32_Rich-clone.jpg", ici, ep );

      Bitmap bmp2 = (Bitmap)Image.FromFile( sourceFile2 );
      bmp2.Save( @"C:\Users\jerin\Desktop\WP_20170321_08_08_23_Rich-clone.jpg", ici, ep );
    }

    static void testCopyPixels() {
      string sourceFile1 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170226_11_16_32_Rich.jpg"; // 5.95MB,6097KB
      string sourceFile2 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170321_08_08_23_Rich.jpg"; // 6.90MB,7068KB

      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      ImageCodecInfo ici = null;

      foreach (ImageCodecInfo codec in codecs) {
        if (codec.MimeType == "image/jpeg")
          ici = codec;
      }

      EncoderParameters ep = new EncoderParameters();
      ep.Param[0] = new EncoderParameter( System.Drawing.Imaging.Encoder.Quality, (long)80 );

      Bitmap bmpSource1 = (Bitmap)Bitmap.FromFile( sourceFile1 );
      Bitmap bmp1 = Vision.Tools.ImageHelper.CopyBitmapSafe( bmpSource1 );
      bmp1.Save( @"C:\Users\jerin\Desktop\WP_20170226_11_16_32_Rich-clone.jpg", ici, ep );
      // 2.64MB

      Bitmap bmpSource2 = (Bitmap)Bitmap.FromFile( sourceFile2 );
      Bitmap bmp2 = Vision.Tools.ImageHelper.CopyBitmapSafe( bmpSource2 );
      bmp2.Save( @"C:\Users\jerin\Desktop\WP_20170321_08_08_23_Rich-clone.jpg", ici, ep );
      // 3.09MB
    }

    static void testCopyPixels2() {
      string sourceFile1 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170226_11_16_32_Rich.jpg"; // 5.95MB,6097KB
      string sourceFile2 = @"E:\OneDrive\Pictures\Camera Roll\WP_20170321_08_08_23_Rich.jpg"; // 6.90MB,7068KB

      Bitmap bmpSource1 = (Bitmap)Bitmap.FromFile( sourceFile1 );
      using (var ms1 = Vision.Tools.ImageHelper.CompressBitmapToStream( bmpSource1 )) {
        using (FileStream fs1 = new FileStream( @"C:\Users\jerin\Desktop\WP_20170226_11_16_32_Rich-clone.jpg", FileMode.Create, FileAccess.Write )) {
          ms1.WriteTo( fs1 );
          fs1.Flush();
          fs1.Close();
        }
      }
      // 2.64MB

      Bitmap bmpSource2 = (Bitmap)Bitmap.FromFile( sourceFile2 );
      using (var ms2 = Vision.Tools.ImageHelper.CompressBitmapToStream( bmpSource2 )) {
        using (FileStream fs2 = new FileStream( @"C:\Users\jerin\Desktop\WP_20170321_08_08_23_Rich-clone.jpg", FileMode.Create, FileAccess.Write )) {
          ms2.WriteTo( fs2 );
          fs2.Flush();
          fs2.Close();
        }
      }
      // 3.09MB
    }

    // failed: IronPython无法处理PIL库中的.pyd文件
    static void testImageHashUsingIronPython() {
      string dir = @"C:\Users\jerin\Desktop\imagehash\";
      string file1 = dir + "imagehash.png";
      string file2 = dir + "lenna.png";
      string file3 = dir + "lenna1.jpg";
      string file4 = dir + "lenna2.jpg";

      Console.WriteLine( file1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash1 = Vision.Runtime.Python.ComputeImageWHash( file1 );
      Console.WriteLine( hash1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash2 = Vision.Runtime.Python.ComputeImageWHash( file2 );
      Console.WriteLine( hash2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash3 = Vision.Runtime.Python.ComputeImageWHash( file3 );
      Console.WriteLine( hash3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash4 = Vision.Runtime.Python.ComputeImageWHash( file4 );
      Console.WriteLine( hash4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
    }

    // worked @ 2017-4-15
    static void testImageHashUsingCommand() {
      string dir = @"C:\Users\jerin\Desktop\imagehash\";
      string file1 = dir + "imagehash.png";
      string file2 = dir + "lenna.png";
      string file3 = dir + "lenna1.jpg";
      string file4 = dir + "lenna2.jpg";

      Console.WriteLine( file1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash1 = Vision.Runtime.PythonCommond.ComputeImageWHash( file1 );
      Console.WriteLine( hash1 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash2 = Vision.Runtime.PythonCommond.ComputeImageWHash( file2 );
      Console.WriteLine( hash2 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash3 = Vision.Runtime.PythonCommond.ComputeImageWHash( file3 );
      Console.WriteLine( hash3 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );

      Console.WriteLine( file4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
      string hash4 = Vision.Runtime.PythonCommond.ComputeImageWHash( file4 );
      Console.WriteLine( hash4 );
      Console.WriteLine( DateTime.Now.ToLongTimeString() );
    }

  }

}
