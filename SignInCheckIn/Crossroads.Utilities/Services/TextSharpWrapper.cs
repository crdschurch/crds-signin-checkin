using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;

namespace Crossroads.Utilities.Services
{
    public class TextSharpWrapper
    {
        // this function is probably not as robust as it could be - we depend on having the field key contained
        // as a merge value in the code that is being passed in - should probably do some exception handling or 
        // error catching here
        public string PopulatePdfMergeFields(string pdfFilepath, Dictionary<string, string> mergeKeys)
        {
            string pdfContent = String.Empty;

            using (var existingFileStream = new FileStream(pdfFilepath, FileMode.Open))
            using (var newFileStream = new MemoryStream())
            {
                // Open existing PDF
                var pdfReader = new PdfReader(existingFileStream);

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
