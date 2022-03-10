
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyFileServices : BaseServices<ZrzyFile>, IZrzyFileServices
    {
        private readonly IBaseRepository<ZrzyFile> _dal;
        public ZrzyFileServices(IBaseRepository<ZrzyFile> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}