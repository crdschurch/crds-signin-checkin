using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.util;
using Crossroads.Utilities.Services.Interfaces;
using iTextSharp.text.pdf;
using System.Diagnostics;

namespace Crossroads.Utilities.Services
{
    public class TextSharpWrapper : IPdfEditor
    {
        // this function is probably not as robust as it could be - we depend on having the field key contained
        // as a merge value in the code that is being passed in - should probably do some exception handling here
        public string PopulatePdfMergeFields(byte[] pdfFile, Dictionary<string, string> mergeKeys)
        {
            string pdfContent = String.Empty;
            Stream stream = new MemoryStream(pdfFile);

            using (var newFileStream = new MemoryStream())
            {
                // Open existing PDF
                var pdfReader = new PdfReader(stream);

                // PdfStamper, which will create the pdf
                var stamper = new PdfStamper(pdfReader, newFileStream);

                var form = stamper.AcroFields;
                var fieldKeys = form.Fields.Keys;

                foreach (string fieldKey in fieldKeys)
                {
                    form.SetField(fieldKey, mergeKeys[fieldKey]);
                }

                stamper.Close();
                pdfContent = Convert.ToBase64String(newFileStream.ToArray());
                pdfReader.Close();
            }

            return pdfContent;
        }
    }
}
