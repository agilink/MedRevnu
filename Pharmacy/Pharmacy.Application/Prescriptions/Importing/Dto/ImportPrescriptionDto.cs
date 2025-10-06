using System;
using Abp.AutoMapper;
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;

namespace ATI.Pharmacy.Importing.Dto;

[AutoMapTo(typeof(Prescription))]
public class ImportPrescriptionDto : ImportFromExcelDto
{
    public int PrescriptionID { get; set; }
    public string Specialty { get; set; }
    public string LicenseNumber { get; set; }

}