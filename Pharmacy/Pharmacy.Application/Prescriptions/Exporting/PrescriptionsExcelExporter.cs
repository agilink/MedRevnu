using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using ATI.DataExporting.Excel.MiniExcel;
using ATI.Pharmacy.Dtos;
using ATI.Dto;
using ATI.Storage;

namespace ATI.Pharmacy.Exporting;

public class PrescriptionsExcelExporter : MiniExcelExcelExporterBase, IPrescriptionsExcelExporter
{
    private readonly ITimeZoneConverter _timeZoneConverter;
    private readonly IAbpSession _abpSession;
    private readonly IPropertyInfoHelper _propertyInfoHelper;

    public PrescriptionsExcelExporter(
        ITimeZoneConverter timeZoneConverter,
        IAbpSession abpSession,
        ITempFileCacheManager tempFileCacheManager, IPropertyInfoHelper propertyInfoHelper) :
base(tempFileCacheManager)
    {
        _timeZoneConverter = timeZoneConverter;
        _abpSession = abpSession;
        _propertyInfoHelper = propertyInfoHelper;
    }

    public FileDto ExportToFile(List<GetPrescriptionForViewDto> Prescriptions, List<string> selectedColumns)
    {

        var items = new List<Dictionary<string, object>>();

        foreach (var PrescriptionForViewDto in Prescriptions)
        {
            var item = PrescriptionForViewDto.Prescription;

            if (selectedColumns is { Count: 0 })
            {
                break;
            }

            var rowItem = new Dictionary<string, object>();

            foreach (var selectedColumn in selectedColumns)
            {
                // if the property is found, it will be added to the list of items
                if (typeof(PrescriptionDto).GetProperty(selectedColumn) is { } property)
                {
                    rowItem.Add(property.Name, _propertyInfoHelper.GetConvertedPropertyValue(property, item, HandleLists) ?? string.Empty);
                }
            }

            items.Add(rowItem);
        }

        return CreateExcelPackage("PrescriptionsList.xlsx", items);

    }

    private static string? HandleLists(PropertyInfo property, object item)
    {
        var propertyType = property.PropertyType;

        if (!typeof(IEnumerable).IsAssignableFrom(propertyType) &&
            !propertyType.IsGenericType &&
            propertyType.GetGenericTypeDefinition() != typeof(List<>))
        {
        }

        var genericType = propertyType.GetGenericArguments()[0];

        // You can change the way the list is handled here
        return string.Empty;
    }

}