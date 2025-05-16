using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using ATI.MultiTenancy.Accounting.Dto;

namespace ATI.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
