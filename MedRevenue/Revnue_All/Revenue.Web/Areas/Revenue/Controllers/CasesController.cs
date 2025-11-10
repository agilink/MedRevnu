using Microsoft.AspNetCore.Mvc;
using ATI.Revenue.Application.Cases;
using ATI.Revenue.Application.Cases.Dtos;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ATI.Revenue.Web.Areas.Revenue.Controllers
{
    [Area("Revenue")]
    public class CasesController : Controller
    {
        private readonly ICasesAppService _casesAppService;

        public CasesController(ICasesAppService casesAppService)
        {
            _casesAppService = casesAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrEditCaseDto input)
        {
            if (ModelState.IsValid)
            {
                await _casesAppService.CreateAsync(input);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var output = await _casesAppService.GetCaseForEdit(new EntityDto<int> { Id = id });
            return View(output.Case);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateOrEditCaseDto input)
        {
            if (ModelState.IsValid)
            {
                await _casesAppService.UpdateAsync(input);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        public async Task<IActionResult> Details(int id)
        {
            var output = await _casesAppService.GetCaseForView(id);
            return View(output.Case);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _casesAppService.DeleteAsync(new EntityDto<int> { Id = id });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetAllCasesInput input)
        {
            var result = await _casesAppService.GetAllFiltered(input);
            return Json(result);
        }
    }
}