using System.Threading.Tasks;
using Abp.Dependency;

namespace ATI.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}