/***************************************************************************************

MIT License

Copyright (c) 2016 Mark Young

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

***************************************************************************************/

// Makes use of IconTools.cs from http://www.brad-smith.info/blog/archives/763
// Thanks!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace t2tIcon2PNG
{
   class Program
   {

      enum ExitCodes
      {
         eSuccess = 0,
         eInvalidNumArgs = 1,
         eFileNotFound = 2,
         eSaveDirInvalid = 4,
         eIconExtractFailed = 8,
         eSavePNGFailed = 16
      };

      static ExitCodes eExitCode = ExitCodes.eSuccess;
      static string sExtractFile = "";
      static string sExportDir = "";
#if DEBUG
      static bool bDebug = true;
#else
       static bool bDebug = false;
#endif

      static void Main(string[] args)
      {
         do
         {

            // Check number of arguaments
            if (args.Length < 2)
            {
               eExitCode = ExitCodes.eInvalidNumArgs;
               Console.WriteLine("Invalid number of arguments supplied.");
               break;
            }
            if (bDebug)
            {
               Console.WriteLine("DEBUG: Params Valid");
            }

            // Is arg 0 a valid file?
            sExtractFile = args[0];
            if (File.Exists(sExtractFile) == false)
            {
               eExitCode = ExitCodes.eFileNotFound;
               Console.WriteLine("File not found, " + sExtractFile);
               break;
            }
            if (bDebug)
            {
               Console.WriteLine("DEBUG: File is: " + sExtractFile);
            }

            // Is arg 1 a valid dir?
            sExportDir = args[1];
            if (Directory.Exists(sExportDir) == false)
            {
               // Try and create it!
               try
               {
                  Directory.CreateDirectory(sExportDir);
                  if (bDebug)
                  {
                     Console.WriteLine("DEBUG: Directory Created: " + sExportDir);
                  }
               }
               catch (Exception eXcep)
               {
                  eExitCode = ExitCodes.eSaveDirInvalid;
                  Console.WriteLine("Directory error, " + eXcep.Message);
                  break;
               }
            }
            if (bDebug)
            {
               Console.WriteLine("DEBUG: Directory is: " + sExportDir);
            }

            // OK, lets get the icon...
            Icon icoExtract;
            try
            {
               icoExtract = IconTools.GetIconForExtension(Path.GetExtension(sExtractFile), ShellIconSize.LargeIcon);
            }
            catch (Exception eXcep)
            {
               eExitCode = ExitCodes.eIconExtractFailed;
               Console.WriteLine("Extract failed, " + eXcep.Message);
               break;
            }
            if (bDebug)
            {
               Console.WriteLine("DEBUG: Icon extracted to resource object");
            }

            string sExtractedFileName = Path.GetExtension(sExtractFile).Substring(1) + "-ico.png";
            string sFullPath = Path.Combine(sExportDir, sExtractedFileName);
            try
            {
               icoExtract.ToBitmap().Save(sFullPath, ImageFormat.Png);
            }
            catch (Exception eXcep)
            {
               eExitCode = ExitCodes.eSavePNGFailed;
               Console.WriteLine("Save PNG failed, " + eXcep.Message);
               break;
            }
            if (bDebug)
            {
               Console.WriteLine("DEBUG: Icon Saved: " + sFullPath);
            }

         } while (false);

         if (bDebug) {
            Console.WriteLine("End of program.  Exit Code: " + eExitCode.ToString() + " (" + ((int)eExitCode).ToString() + ").");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
         }

         Environment.Exit((int)eExitCode);
      }
   }
}
