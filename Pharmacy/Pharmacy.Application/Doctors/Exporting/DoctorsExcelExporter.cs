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

public class DoctorsExcelExporter : MiniExcelExcelExporterBase, IDoctorsExcelExporter
{
    private readonly ITimeZoneConverter _timeZoneConverter;
    private readonly IAbpSession _abpSession;
    private readonly IPropertyInfoHelper _propertyInfoHelper;

    public DoctorsExcelExporter(
        ITimeZoneConverter timeZoneConverter,
        IAbpSession abpSession,
        ITempFileCacheManager tempFileCacheManager, IPropertyInfoHelper propertyInfoHelper) :
base(tempFileCacheManager)
    {
        _timeZoneConverter = timeZoneConverter;
        _abpSession = abpSession;
        _propertyInfoHelper = propertyInfoHelper;
    }

    public FileDto ExportToFile(List<GetDoctorForViewDto> doctors, List<string> selectedColumns)
    {

        var items = new List<Dictionary<string, object>>();

        foreach (var doctorForViewDto in doctors)
        {
            var item = doctorForViewDto.Doctor;

            if (selectedColumns is { Count: 0 })
            {
                break;
            }

            var rowItem = new Dictionary<string, object>();

            foreach (var selectedColumn in selectedColumns)
            {
                // if the property is found, it will be added to the list of items
                if (typeof(DoctorDto).GetProperty(selectedColumn) is { } property)
                {
                    rowItem.Add(property.Name, _propertyInfoHelper.GetConvertedPropertyValue(property, item, HandleLists) ?? string.Empty);
                }
            }

            items.Add(rowItem);
        }

        return CreateExcelPackage("DoctorsList.xlsx", items);

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