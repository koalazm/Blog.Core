
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyMapServerServices : BaseServices<ZrzyMapServer>, IZrzyMapServerServices
    {
        private readonly IBaseRepository<ZrzyMapServer> _dal;
        public ZrzyMapServerServices(IBaseRepository<ZrzyMapServer> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}