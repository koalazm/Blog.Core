
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyProjectMapServerServices : BaseServices<ZrzyProjectMapServer>, IZrzyProjectMapServerServices
    {
        private readonly IBaseRepository<ZrzyProjectMapServer> _dal;
        public ZrzyProjectMapServerServices(IBaseRepository<ZrzyProjectMapServer> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}