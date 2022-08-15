/* The PDFMerge is a console application that will take 2 paramters 
 * param1:source files eaxample C:\temp\abc*.pdf will merge all files with matching
 * the abc and * is a wild card
 * Param2: destination file for the merged PDFs
 * The source files will be deleted once the files are merged.
 * 
 * Uses ITEXT PDF core library from iText Software under the opensource license
 * Several iText engineers are actively supporting the project on StackOverflow: https://stackoverflow.com/questions/tagged/itext
 * Source code is availble from https://itextpdf.com
 * 
 * 
 * Copyright (C) < 2022 >  < John Reynolds>  johnr@cambrio.com
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PDFMerge
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// Usage: PDFMerge.exe {sourceFilefilter} {DestinationFile}
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(
                 @" PDFMerge will take a wild card paramter for PDF file sto merge and 
                 a second paramter that will be the merged PDF file as the output
                 PDFMerge.exe Copyright(C) < 2022 >  <John Reynolds> 

                 This program is free software: you can redistribute it and / or modify
                 it under the terms of the GNU Affero General Public License as published
                 by the Free Software Foundation, either version 3 of the License, or
                 (at your option) any later version.

                 This program is distributed in the hope that it will be useful,
                 but WITHOUT ANY WARRANTY; without even the implied warranty of
                 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
                 GNU Affero General Public License for more details.

                 You should have received a copy of the GNU Affero General Public License
                 along with this program.If not, see < https://www.gnu.org/licenses/>.");


                if (args.Length < 2)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    
                    switch (key.KeyChar.ToString().ToUpper())
                    {
                        case "W":
                            System.Diagnostics.Process.Start("https://www.gnu.org/licenses/agpl-3.0.html");
                            break;
                        case "C":
                            System.Diagnostics.Process.Start("https://www.gnu.org/licenses/agpl-3.0.html");
                            break;
                    }


                    MessageBox.Show($"No Source file or Destination File {Environment.NewLine}" +
                        "Usage: PDFMerge.exe {sourceFilefilter} {DestinationFile}  " + Environment.NewLine +
                        "Example: PDFMerge.exe \"c:\\temp\\ABC*.PDF\" \"c:\\temp\\abc.pdf\"");
                    return;
                }
//#if DEBUG
                bool waiting = true;
                while (waiting)
                {
                    ConsoleKeyInfo key1 = Console.ReadKey();
                    waiting = false;
                }
//#endif
                {
                    Thread.Sleep(2000);
                }
                string sourceFilesFilter = args[0];
                string targetFile = args[1];
                HelperClass.MergePDF(sourceFilesFilter, targetFile);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                Console.WriteLine("Press ESC to stop");
                //do
                //{
                //    while (!Console.KeyAvailable)
                //    {
                //        // Do something
                //    }
                //} while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
        }



    }
}
