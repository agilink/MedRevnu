using System;
using Abp.AutoMapper;
using ATI.DataImporting.Excel;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;

namespace ATI.Pharmacy.Importing.Dto;

[AutoMapTo(typeof(Doctor))]
public class ImportDoctorDto : ImportFromExcelDto
{
    public int DoctorID { get; set; }
    public string Specialty { get; set; }
    public string LicenseNumber { get; set; }

}