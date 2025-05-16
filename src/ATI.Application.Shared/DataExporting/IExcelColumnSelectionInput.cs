using System.Collections.Generic;

namespace ATI.DataExporting
{
    public interface IExcelColumnSelectionInput
    {
        List<string> SelectedColumns { get; set; }
    }
}