using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using ATI.Authorization;
using ATI.Revenue.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    [AbpMvcAuthorize(AppPermissions.Pages_Revenue)]
    public class HomeController : Controller
    {
        private readonly IRepository<ProcedureTransaction, int> _procedureTransactionRepository;
        private readonly IRepository<Product, int> _productRepository;

        public HomeController(
            IRepository<ProcedureTransaction, int> procedureTransactionRepository,
            IRepository<Product, int> productRepository)
        {
            _procedureTransactionRepository = procedureTransactionRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Summary tiles for the current year.
        /// </summary>
        /// <remarks>
        /// These were hardcoded to 150 cases, 75 products and $250,000 of revenue, so the
        /// page showed the same invented figures no matter what was in the database. They
        /// now come from ProcedureTransaction and Product.
        /// </remarks>
        public async Task<IActionResult> Dashboard()
        {
            var year = Abp.Timing.Clock.Now.Year;

            var transactions = _procedureTransactionRepository.GetAll()
                .Where(t => t.ProcedureDate.Year == year);

            // A case is one transaction row; summing device quantities would
            // report a multi-device procedure as several cases.
            ViewBag.CasesCount = await transactions.CountAsync();
            ViewBag.Revenue = await transactions.SumAsync(t => (decimal?)t.TotalAmount) ?? 0m;
            ViewBag.ProductsCount = await _productRepository.CountAsync(p => p.IsActive);
            ViewBag.Year = year;

            return View();
        }
    }
}
