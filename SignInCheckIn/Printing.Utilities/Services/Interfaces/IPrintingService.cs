using Printing.Utilities.Models;

namespace Printing.Utilities.Services.Interfaces
{
    public interface IPrintingService
    {
        int SendPrintRequest(PrintRequestDto printRequestDto);
    }
}
