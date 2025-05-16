using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using ATI.Authorization;
using ATI.EntityChanges;
using ATI.EntityChanges.Dto;
using ATI.Web.Areas.Core.Models.EntityChanges;
using ATI.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_EntityChanges_FullHistory)]
    public class EntityChangesController : ATIControllerBase
    {
        private readonly IEntityChangeAppService _entityChangeAppService;

        public EntityChangesController(IEntityChangeAppService entityChangeAppService)
        {
            _entityChangeAppService = entityChangeAppService;
        }

        [HttpGet]
        [Route("/Core/EntityChanges/{entityId}/{entityTypeFullName}")]
        public async Task<IActionResult> Index(string entityId, string entityTypeFullName)
        {
            var entityChanges = await _entityChangeAppService.GetEntityChangesByEntity(new GetEntityChangesByEntityInput
            {
                EntityId = entityId,
                EntityTypeFullName = entityTypeFullName,
            });

            ViewBag.ChangesCount = entityChanges.Items.Count;
            ViewBag.EntityTypeShortName = entityTypeFullName.Substring(entityTypeFullName.LastIndexOf('.') + 1);
            ViewBag.EntityId = entityId;

            var viewModel = new EntityChangeListViewModel
            {
                EntityAndPropertyChanges = ObjectMapper.Map<List<EntityAndPropertyChangeListDto>>(entityChanges.Items)
            };

            return View(viewModel);
        }
    }
}
