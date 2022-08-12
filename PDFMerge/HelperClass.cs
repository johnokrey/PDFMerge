/* The PDFMerge is a console application that will take 2 paramters 
 * param1:source files eaxample C:\temp\abc*.pdf will merge all files with matching
 * the abc and * is a wild card
 * Param2: destination file for the merged PDFs
 * The source files will be deleted once the files are merged.
 * The utility uses the ITEXT library under the APGL license  
 * 
 * Uses ITEXT PDF core library from iText Software under the opensource license
 * Several iText engineers are actively supporting the project on StackOverflow: https://stackoverflow.com/questions/tagged/itext
 * Source code is availble from https://itextpdf.com
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;

namespace PDFMerge
{
    internal class HelperClass
    {
        /// <summary>
        /// Will check if the file has finished writing in case the 
        /// PDF creation is doen in another thread.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool IsFileLocked(FileInfo file)
        {
            FileStream fstream = null;

            try
            {
                fstream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
            }

            //file is not locked
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SourceFileFilter"></param>
        /// this paramter is the poath + the file filter with 
        /// Example: "C:\temp\abc*.pdf"
        /// <param name="MergedFilenanme"></param>
        /// This is the file in which all PDF's found wil be merged into
        /// The file will be saved and teh source files will be removed from disk once it is 
        /// successfully combined
        /// It will add a water ark into the reports
        public static void MergePDF(string SourceFileFilter, string MergedFilenanme)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            List<String> LogList = new List<String>();
        
            try
            {
                try
                {
                    File.Delete(MergedFilenanme);
                }
                catch { }
                
                Thread.Sleep(2000);

                string fileFilter = Path.GetFileName(SourceFileFilter);

                DirectoryInfo di = new DirectoryInfo($"{Path.GetDirectoryName(SourceFileFilter)}");

                FileInfo[] PDFfiles = di.GetFiles(fileFilter);

                string outputPdfPath = MergedFilenanme;

                sourceDocument = new Document();

                pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                //output file Open  
                sourceDocument.Open();

                bool loaded = false;
                LogList.Add("Start PDF Files Merge");
                //files list wise Loop  
                foreach (var file in PDFfiles)
                {
                    //int pages = TotalPageCount(fileArray[f]);
                    var f = new FileInfo(file.FullName);
                    Console.WriteLine($"Merge {file.FullName} into {outputPdfPath}");
                    LogList.Add($"Merge {file.FullName} into {outputPdfPath}");

                    //While File is not accesable because of writing process
                    while (IsFileLocked(f))
                    {
                        Thread.Sleep(2000);
                        Console.WriteLine($"File {file.FullName} Locked - Wait");
                        LogList.Add($"File {file.FullName} Locked - Wait" );

                    }

                    LogList.Add($"File {file.FullName} File Ready to Merge");
                    Thread.Sleep(1000);

                    reader = new PdfReader(file.FullName);

                    //PdfContentByte watermark;

                    //Add pages in new file  
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }
                    LogList.Add($"{file.FullName} WaterMark Added for {reader.NumberOfPages} pages");
                    reader.Close();
                    LogList.Add("All Files merged !!!");
                }

                
                //save the output file  
                sourceDocument.Close();

                //byte[] pdfbyte =     WriteToPdf(outputPdfPath, "ITextSharp PDF Library");
                try
                {

                    string watermarkText = "ITEXT Copyright APGL License";
                    float fontSize = 50;
                    float xPosition = 300;
                    float yPosition = 400;
                    float angle = 45;

                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);

                    Document document = new Document();
                    PdfReader pdfReader = new PdfReader(outputPdfPath);
                    PdfStamper stamp = new PdfStamper(pdfReader, new FileStream(outputPdfPath.ToUpper().Replace(".PDF", "[temp][file].pdf"), FileMode.Create));

                    var gstate = new PdfGState { FillOpacity = 0.1f, StrokeOpacity = 0.3f };

                    PdfContentByte waterMark;
                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    {
                        try
                        {
                            waterMark = stamp.GetOverContent(page);
                            waterMark.BeginText();
                            waterMark.SetColorFill(iTextSharp.text.pdf.CMYKColor.LIGHT_GRAY);
                            waterMark.SetGState(gstate);
                            waterMark.SetFontAndSize(baseFont, fontSize);
                            waterMark.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, xPosition, yPosition, angle);
                            waterMark.EndText();

                            waterMark.Add(waterMark);
                        }
                        catch (Exception ex)
                        {
                            LogList.Add(ex.Message);
                        }
                    }

                    stamp.FormFlattening = true;
                    stamp.Close();
                    pdfReader.Close();
                    // now delete the original file and rename the temp file to the original file
                    File.Delete(outputPdfPath);
                    
                    File.Move(outputPdfPath.ToUpper().Replace(".PDF", "[temp][file].pdf"), outputPdfPath);

                    foreach (var file in PDFfiles)
                    {
                        Console.WriteLine("Deleting File: "+file.Name);
                        File.Delete(file.FullName);
                    }
                }


                catch (Exception ex)
                {
                    LogList.Add(ex.Message);
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            catch ( Exception ex1 )
            {
                LogList.Add(ex1.Message);
                MessageBox.Show(ex1.Message);
                return;
            }
            finally
            {
                string fileName = Path.Combine(Path.GetDirectoryName(MergedFilenanme),"PDFMerge.Log");
                File.Delete(fileName);
                FileStream fs = new FileStream(fileName, FileMode.CreateNew);

                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {

                    foreach (var str in LogList)
                    {
                        writer.WriteLine(str);
                    }
                }
                fs.Close();
            }
        }
    }
}
