
using Blog.Core.IServices;
using Blog.Core.Model.Models;
using Blog.Core.Services.BASE;
using Blog.Core.IRepository.Base;
using Blog.Core.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Blog.Core.Model.ViewModels;
using Blog.Core.IRepository;

namespace Blog.Core.Services
{
    public class ZrzyProjectUserServices : BaseServices<ZrzyProjectUser>, IZrzyProjectUserServices
    {

        private readonly IZrzyProjectUserRepository _dal;
        private readonly IBaseRepository<ZrzyProject> _projectDal;
        private readonly IBaseRepository<sysUserInfo> _userDal;
        public ZrzyProjectUserServices(IZrzyProjectUserRepository dal,
            IBaseRepository<ZrzyProject> projectDal,
            IBaseRepository<sysUserInfo> userDal)
        {
            this._dal = dal;
            this._projectDal = projectDal;
            this._userDal = userDal;
            base.BaseDal = dal;
        }

        /// <summary>
        /// 获取全部 角色接口(按钮)关系数据
        /// </summary>
        /// <returns></returns>
        [Caching(AbsoluteExpiration = 10)]
        public async Task<List<ZrzyProjectUser>> GetProjectUser()
        {
            var projectUsers = await base.Query(a => a.IsDeleted == false);
            var projects = await _projectDal.Query(a => a.IsDeleted == false);
            var users = await _userDal.Query(a => a.tdIsDelete == false);

            //var roleModulePermissionsAsync = base.Query(a => a.IsDeleted == false);
            //var rolesAsync = _roleRepository.Query(a => a.IsDeleted == false);
            //var modulesAsync = _moduleRepository.Query(a => a.IsDeleted == false);

            //var roleModulePermissions = await roleModulePermissionsAsync;
            //var roles = await rolesAsync;
            //var modules = await modulesAsync;


            if (projectUsers.Count > 0)
            {
                foreach (var item in projectUsers)
                {
                    item.Project = projects.FirstOrDefault(d => d.Id == item.ProjectId);
                    item.User = users.FirstOrDefault(d => d.uID == item.UserId);
                }

            }
            return projectUsers;
        }
        public async Task<List<ZrzyProjectUserDto>> QueryMuchTable()
        {
            return await _dal.QueryMuchTable();
        }

        public async Task<List<ZrzyProjectUser>> ProjectUserMaps()
        {
            return await _dal.ProjectUserMaps();
        }

        /// <summary>
        /// 批量更新菜单与接口的关系
        /// </summary>
        /// <param name="permissionId">菜单主键</param>
        /// <param name="moduleId">接口主键</param>
        /// <returns></returns>
        public async Task UpdateUserId(int permissionId, int moduleId)
        {
            await _dal.UpdateUserId(permissionId, moduleId);
        }

    }
}