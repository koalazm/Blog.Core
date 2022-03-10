
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyProjectServices : BaseServices<ZrzyProject>, IZrzyProjectServices
    {
        private readonly IBaseRepository<ZrzyProject> _dal;
        public ZrzyProjectServices(IBaseRepository<ZrzyProject> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}