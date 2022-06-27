using B2B.Dao;
using B2B.Dao.Contexts;
using B2B.Dao.Models;
using B2B.Service.Interfacies;
using B2B.Service.Models;

namespace B2B.Service
{
    public class EvaUserService : IEvaUserService
    {
        private IUnitOfWork _uow;

        public EvaUserService(B2BDbContext dbContext)
        {
            _uow = new UnitOfWork(dbContext);
        }

        public IEnumerable<Twb2bSysmenu> GetMenuList(SysMenuFind? find = null)
        {
            var menuRepo = _uow.Repository<Twb2bSysmenu>();

            var list = menuRepo.GetAll().AsEnumerable();

            return list;
        }
    }
}