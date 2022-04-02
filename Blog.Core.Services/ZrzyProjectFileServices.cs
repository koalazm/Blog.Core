
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyProjectFileServices : BaseServices<ZrzyProjectFile>, IZrzyProjectFileServices
    {
        private readonly IBaseRepository<ZrzyProjectFile> _dal;
        public ZrzyProjectFileServices(IBaseRepository<ZrzyProjectFile> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}