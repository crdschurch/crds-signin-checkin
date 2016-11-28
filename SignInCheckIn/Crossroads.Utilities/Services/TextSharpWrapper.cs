using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Crossroads.Utilities.Services.Interfaces;
using iTextSharp.text.pdf;

namespace Crossroads.Utilities.Services
{
    public class TextSharpWrapper : IPdfEditor
    {
        // this function is probably not as robust as it could be - we depend on having the field key contained
        // as a merge value in the code that is being passed in - should probably do some exception handling here
        public string PopulatePdfMergeFields(byte[] pdfFile, Dictionary<string, string> mergeKeys)
        {
            string pdfContent;
            var stream = new MemoryStream(pdfFile);

            using (var newFileStream = new MemoryStream())
            {
                // Open existing PDF
                var pdfReader = new PdfReader(stream);

                // PdfStamper, which will create the pdf
                var stamper = new PdfStamper(pdfReader, newFileStream);

                var form = stamper.AcroFields;
                form.Fields.Keys.ToList().Where(mergeKeys.ContainsKey).ToList().ForEach(k => form.SetField(k, mergeKeys[k]));

                stamper.Close();
                pdfContent = Convert.ToBase64String(newFileStream.ToArray());
                pdfReader.Close();
            }

            return pdfContent;
        }
    }
}
