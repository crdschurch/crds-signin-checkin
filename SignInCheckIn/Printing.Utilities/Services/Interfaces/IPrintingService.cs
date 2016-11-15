using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printing.Utilities.Models;

namespace Printing.Utilities.Services.Interfaces
{
    public interface IPrintingService
    {
        int SendPrintRequest(PrintRequestDto printRequestDto);
    }
}
