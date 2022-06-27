using B2B.Dao;
using B2B.Dao.Contexts;
using B2B.Dao.Models;
using Microsoft.AspNetCore.Mvc;

namespace B2B.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IUnitOfWork _uow;

        public TestController(B2BDbContext dbContext)
        {
            _uow = new UnitOfWork(dbContext);
        }

        [HttpGet(Name = "GetMenu")]
        public IEnumerable<Twb2bSysmenu> Get()
        {
            var repo = _uow.Repository<Twb2bSysmenu>();
            var data = repo.GetAll().ToList();

            return data;
        }

    }
}
