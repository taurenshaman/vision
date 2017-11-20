using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Runtime {
  public static class PythonCommond {
    const string PythonExecutePath = @"D:\Python27\python.exe";


    public static string Excute( string PythonExecutePathint, string pythonScriptPath, int millisecondsWaitForExit, params string[] parameters) {
      StringBuilder sbParams = new StringBuilder();
      if(parameters != null) {
        foreach (string param in parameters)
          sbParams.Append( " " + param );
      }

      string output = "";     //输出字符串

      ProcessStartInfo startInfo = new ProcessStartInfo {
        FileName = PythonExecutePath,  //设定需要执行的命令 "python "
        Arguments = string.Format( "{0} {1}", pythonScriptPath, sbParams.ToString() )
      };
      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardInput = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.CreateNoWindow = true;

      using (Process process = new Process()) {
        process.StartInfo = startInfo;
        //process.OutputDataReceived += (s, e) => {
        //  System.Diagnostics.Debug.WriteLine( e.Data );
        //};
        try {
          if (process.Start())       //开始进程
          {
            if (millisecondsWaitForExit == 0)
              process.WaitForExit();     //这里无限等待进程结束
            else
              process.WaitForExit( millisecondsWaitForExit );  //这里等待进程结束，等待时间为指定的毫秒
            output = process.StandardOutput.ReadToEnd();//读取进程的输出
          }
        }
        catch (Exception ex) {
          Debug.WriteLine( ex.Message );
        }
        finally {
          if (process != null)
            process.Close();
        }
      }

      return output;
    }

    public static async Task<string> ComputeImageWHash(string PythonExecutePath, string pythonScriptPath_ImageHash, string imagePath, int millisecondsWaitForExit = 0) {
      //string output = Excute( PythonExecutePath, pythonScriptPath_ImageHash, millisecondsWaitForExit, imagePath );
      //return output;

      string r = await Task.Run<string>( () => {
        string output = Excute( PythonExecutePath, pythonScriptPath_ImageHash, millisecondsWaitForExit, imagePath );
        return output;
      } );
      return r;
    }

  }

}
