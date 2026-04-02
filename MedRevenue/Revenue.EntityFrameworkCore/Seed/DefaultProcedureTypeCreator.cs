using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ATI.Revenue.EntityFrameworkCore.Seed
{
    public class DefaultProcedureTypeCreator
    {
        private readonly RevenueModuleDbContext _context;

        public DefaultProcedureTypeCreator(RevenueModuleDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateProcedureTypes();
        }

        private void CreateProcedureTypes()
        {
            var procedureTypes = new List<ProcedureType>
            {
                // Pacemaker (CategoryGroup = 1)
                new ProcedureType
                {
                    Code = "PM-SC",
                    Name = "Single Chamber Pacemaker",
                    CategoryGroup = CategoryGroup.Pacemaker,
                    Description = "Single chamber pacemaker implantation",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new ProcedureType
                {
                    Code = "PM-DC",
                    Name = "Dual Chamber Pacemaker",
                    CategoryGroup = CategoryGroup.Pacemaker,
                    Description = "Dual chamber pacemaker implantation",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new ProcedureType
                {
                    Code = "PM-DCLB",
                    Name = "Dual Chamber Left Bundle Pacemaker",
                    CategoryGroup = CategoryGroup.Pacemaker,
                    Description = "Dual chamber left bundle branch pacing",
                    IsActive = true,
                    DisplayOrder = 3
                },
                new ProcedureType
                {
                    Code = "PM-CRTP",
                    Name = "CRT-P",
                    CategoryGroup = CategoryGroup.Pacemaker,
                    Description = "Cardiac resynchronization therapy pacemaker",
                    IsActive = true,
                    DisplayOrder = 4
                },
                new ProcedureType
                {
                    Code = "PM-CRTPLB",
                    Name = "CRT-P Left Bundle",
                    CategoryGroup = CategoryGroup.Pacemaker,
                    Description = "Cardiac resynchronization therapy pacemaker with left bundle branch pacing",
                    IsActive = true,
                    DisplayOrder = 5
                },

                // Defibrillator (CategoryGroup = 2)
                new ProcedureType
                {
                    Code = "ICD-SC",
                    Name = "Single Chamber ICD",
                    CategoryGroup = CategoryGroup.Defibrillator,
                    Description = "Single chamber implantable cardioverter-defibrillator",
                    IsActive = true,
                    DisplayOrder = 6
                },
                new ProcedureType
                {
                    Code = "ICD-DC",
                    Name = "Dual Chamber ICD",
                    CategoryGroup = CategoryGroup.Defibrillator,
                    Description = "Dual chamber implantable cardioverter-defibrillator",
                    IsActive = true,
                    DisplayOrder = 7
                },
                new ProcedureType
                {
                    Code = "ICD-CRTD",
                    Name = "CRT-D",
                    CategoryGroup = CategoryGroup.Defibrillator,
                    Description = "Cardiac resynchronization therapy defibrillator",
                    IsActive = true,
                    DisplayOrder = 8
                },
                new ProcedureType
                {
                    Code = "ICD-CRTDLB",
                    Name = "CRT-D Left Bundle",
                    CategoryGroup = CategoryGroup.Defibrillator,
                    Description = "Cardiac resynchronization therapy defibrillator with left bundle branch pacing",
                    IsActive = true,
                    DisplayOrder = 9
                },

                // Battery Change - Pacemaker (CategoryGroup = 3)
                new ProcedureType
                {
                    Code = "BC-PM-SC",
                    Name = "Single Chamber Pacemaker Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangePacemaker,
                    Description = "Single chamber pacemaker generator replacement",
                    IsActive = true,
                    DisplayOrder = 10
                },
                new ProcedureType
                {
                    Code = "BC-PM-DC",
                    Name = "Dual Chamber Pacemaker Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangePacemaker,
                    Description = "Dual chamber pacemaker generator replacement",
                    IsActive = true,
                    DisplayOrder = 11
                },
                new ProcedureType
                {
                    Code = "BC-PM-CRTP",
                    Name = "CRT-P Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangePacemaker,
                    Description = "CRT-P generator replacement",
                    IsActive = true,
                    DisplayOrder = 12
                },

                // Battery Change - ICD (CategoryGroup = 4)
                new ProcedureType
                {
                    Code = "BC-ICD-SC",
                    Name = "Single Chamber ICD Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangeICD,
                    Description = "Single chamber ICD generator replacement",
                    IsActive = true,
                    DisplayOrder = 13
                },
                new ProcedureType
                {
                    Code = "BC-ICD-DC",
                    Name = "Dual Chamber ICD Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangeICD,
                    Description = "Dual chamber ICD generator replacement",
                    IsActive = true,
                    DisplayOrder = 14
                },
                new ProcedureType
                {
                    Code = "BC-ICD-CRTD",
                    Name = "CRT-D Gen Change",
                    CategoryGroup = CategoryGroup.BatteryChangeICD,
                    Description = "CRT-D generator replacement",
                    IsActive = true,
                    DisplayOrder = 15
                },

                // Leadless (CategoryGroup = 5)
                new ProcedureType
                {
                    Code = "LL-AR",
                    Name = "Leadless AR",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker - AR configuration",
                    IsActive = true,
                    DisplayOrder = 16
                },
                new ProcedureType
                {
                    Code = "LL-VR",
                    Name = "Leadless VR",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker - VR configuration",
                    IsActive = true,
                    DisplayOrder = 17
                },
                new ProcedureType
                {
                    Code = "LL-DR",
                    Name = "Leadless DR",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker - DR configuration",
                    IsActive = true,
                    DisplayOrder = 18
                },
                new ProcedureType
                {
                    Code = "LL-ARGC",
                    Name = "Leadless AR Gen Change",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker AR generator replacement",
                    IsActive = true,
                    DisplayOrder = 19
                },
                new ProcedureType
                {
                    Code = "LL-DRGC",
                    Name = "Leadless DR Gen Change",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker DR generator replacement",
                    IsActive = true,
                    DisplayOrder = 20
                },
                new ProcedureType
                {
                    Code = "LL-VRGC",
                    Name = "Leadless VR Gen Change",
                    CategoryGroup = CategoryGroup.Leadless,
                    Description = "Leadless pacemaker VR generator replacement",
                    IsActive = true,
                    DisplayOrder = 21
                }
            };

            foreach (var procedureType in procedureTypes)
            {
                if (!_context.ProcedureTypes.Any(pt => pt.Code == procedureType.Code))
                {
                    _context.ProcedureTypes.Add(procedureType);
                }
            }

            _context.SaveChanges();
        }
    }
}
