using B2B.API.Attributes;
using B2B.Dao.Models;
using B2B.Service.Interfacies;
using Microsoft.AspNetCore.Mvc;

namespace B2B.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiLog]
    public class EvaUserController : ControllerBase
    {
        private readonly ILogger<EvaUserController> _logger;
        private IEvaUserService _euService;

        public EvaUserController(ILogger<EvaUserController> logger, IEvaUserService euService)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into EvaUserController");

            _euService = euService;
        }

        [HttpPost]
        [Route("EU004")]
        public IEnumerable<Twb2bSysmenu> GetMenuList()
        {
            _logger.LogInformation("Hello, this is the menu list!");
            return _euService.GetMenuList();
        }
    }
}
