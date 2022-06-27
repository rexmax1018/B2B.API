using B2B.Dao.Models;
using B2B.Service.Interfacies;
using Microsoft.AspNetCore.Mvc;

namespace B2B.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaUserController : ControllerBase
    {
        private IEvaUserService _euService;

        public EvaUserController(IEvaUserService euService)
        {
            _euService = euService;
        }

        [HttpPost]
        [Route("EU004")]
        public IEnumerable<Twb2bSysmenu> GetMenuList()
        {
            return _euService.GetMenuList();
        }
    }
}
