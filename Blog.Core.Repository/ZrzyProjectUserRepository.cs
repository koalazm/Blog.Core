using Blog.Core.IRepository;
using Blog.Core.IRepository.UnitOfWork;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Blog.Core.Repository.Base;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Core.Repository
{
	/// <summary>
	/// zrzyprojectuserRepository
	/// </summary>
    public class ZrzyProjectUserRepository : BaseRepository<ZrzyProjectUser>, IZrzyProjectUserRepository
    {
        public ZrzyProjectUserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<List<ZrzyProjectUserDto>> QueryMuchTable()
        {
            return await QueryMuch<ZrzyProjectUser, ZrzyProject, sysUserInfo, ZrzyProjectUserDto>(
                (up, p, u) => new object[] {
                    JoinType.Left, up.Id == p.Id,
                    JoinType.Left,  up.UserId == u.uID
                },

                (up, p, u) => new ZrzyProjectUserDto()
                {
                    UserName = u.uRealName,
                    ProjectName = p.ProjectName,
                    UserId = up.UserId,
                    ProjectId = up.Id,
                    UserProjectId = up.Id
                },

                (up, p, u) => up.IsDeleted == false
                );
        }

        public async Task<List<ZrzyProjectUserDto>> QueryMuchTable(int pId)
        {
            return await QueryMuch<ZrzyProjectUser, ZrzyProject, sysUserInfo, ZrzyProjectUserDto>(
                (up, p, u) => new object[] {
                    JoinType.Left, up.Id == p.Id,
                    JoinType.Left,  up.UserId == u.uID
                },

                (up, p, u) => new ZrzyProjectUserDto()
                {
                    UserName = u.uRealName,
                    ProjectName = p.ProjectName,
                    UserId = up.UserId,
                    ProjectId = up.Id,
                    UserProjectId = up.Id
                },

                (up, p, u) => up.IsDeleted == false && p.Id == pId
                );
        }


        public async Task<List<ZrzyProjectUser>> ProjectUserMaps()
        {
            return await QueryMuch<ZrzyProjectUser, ZrzyProject, sysUserInfo, ZrzyProjectUser>(
                (up, p, u) => new object[] {
                    JoinType.Left, up.Id == p.Id,
                    JoinType.Left,  up.UserId == u.uID
                },

                (up, p, u) => new ZrzyProjectUser()
                {
                    Project = p,
                    User = u,
                    IsDeleted = up.IsDeleted
                },

                (up, p, u) => up.IsDeleted == false && p.IsDeleted == false
                );
        }

        public async Task<List<ZrzyProjectUser>> ProjectUserMaps(int pId)
        {
            return await QueryMuch<ZrzyProjectUser, ZrzyProject, sysUserInfo, ZrzyProjectUser>(
                (up, p, u) => new object[] {
                    JoinType.Left, up.Id == p.Id,
                    JoinType.Left,  up.UserId == u.uID
                },

                (up, p, u) => new ZrzyProjectUser()
                {
                    Project = p,
                    User = u,
                    IsDeleted = up.IsDeleted
                },

                (up, p, u) => up.IsDeleted == false && p.IsDeleted == false && p.Id == pId
                );
        }

        public async Task UpdateUserId(int uId, int pId)
        {
            await Db.Updateable<ZrzyProjectUser>(it => it.ProjectId == pId).Where(
                it => it.UserId == uId).ExecuteCommandAsync();
        }
    }
}