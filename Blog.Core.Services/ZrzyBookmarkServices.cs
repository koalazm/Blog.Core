
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;

namespace Blog.Core.Services
{
    public class ZrzyBookmarkServices : BaseServices<ZrzyBookmark>, IZrzyBookmarkServices
    {
        private readonly IBaseRepository<ZrzyBookmark> _dal;
        public ZrzyBookmarkServices(IBaseRepository<ZrzyBookmark> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }
    }
}