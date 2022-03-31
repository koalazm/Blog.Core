using Blog.Core.IRepository.Base;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Core.IRepository
{
	/// <summary>
	/// IzrzyprojectuserRepository
	/// </summary>	
    public interface IZrzyProjectUserRepository : IBaseRepository<ZrzyProjectUser>
    {
        Task<List<ZrzyProjectUserDto>> QueryMuchTable();
        Task<List<ZrzyProjectUserDto>> QueryMuchTable(int pId);
        Task<List<ZrzyProjectUser>> ProjectUserMaps();
        Task<List<ZrzyProjectUser>> ProjectUserMaps(int pId);
        Task UpdateUserId(int uId, int pId);
    }
}