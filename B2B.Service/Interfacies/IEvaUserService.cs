using B2B.Dao.Models;
using B2B.Service.Models;

namespace B2B.Service.Interfacies
{
    public interface IEvaUserService
    {
        IEnumerable<Twb2bSysmenu> GetMenuList(SysMenuFind? find = null);
    }
}
