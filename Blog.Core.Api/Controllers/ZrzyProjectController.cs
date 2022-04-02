using Blog.Core.Common.HttpContextUser;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Blog.Core.AuthHelper;

namespace Blog.Core.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Permissions.Name)]
    public class ZrzyProjectController : ControllerBase
    {
        /// <summary>
        /// 服务器接口，因为是模板生成，所以首字母是大写的，自己可以重构下
        /// </summary>
        private readonly IZrzyProjectServices _zrzyProjectServices;
        private readonly IZrzyProjectUserServices _zrzyProjectUserServices;
        private readonly IZrzyProjectFileServices _zrzyProjectFileServices;
        private readonly IZrzyProjectMapServerServices _zrzyProjectMapServerServices;
        private readonly IZrzyFileServices _zrzyFileServices;
        private readonly IZrzyMapServerServices _zrzyMapServerServices;
        private readonly ISysUserInfoServices _sysUserInfoServices;
        private readonly IUser _user;
        private readonly PermissionRequirement _requirement;

        public ZrzyProjectController(IZrzyProjectServices zrzyProjectServices,
            IZrzyProjectUserServices zrzyProjectUserServices,
            IZrzyProjectFileServices zrzyProjectFileServices,
            IZrzyProjectMapServerServices zrzyProjectMapServerServices,
            IZrzyFileServices zrzyFileServices,
            IZrzyMapServerServices zrzyMapServerServices,
            ISysUserInfoServices sysUserInfoServices,
            IUser user,
            PermissionRequirement requirement)
        {
            _zrzyProjectServices = zrzyProjectServices;
            _zrzyProjectUserServices = zrzyProjectUserServices;
            _zrzyProjectFileServices = zrzyProjectFileServices;
            _zrzyProjectMapServerServices = zrzyProjectMapServerServices;
            _zrzyFileServices = zrzyFileServices;
            _zrzyMapServerServices = zrzyMapServerServices;
            _sysUserInfoServices = sysUserInfoServices;
            _user = user;
            _requirement = requirement;
        }

        [HttpGet]
        public async Task<MessageModel<PageModel<ZrzyProject>>> Get(int page = 1, string key = "", int intPageSize = 50)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            Expression<Func<ZrzyProject, bool>> whereExpression = a => true;

            return new MessageModel<PageModel<ZrzyProject>>()
            {
                msg = "获取成功",
                success = true,
                response = await _zrzyProjectServices.QueryPage(whereExpression, page, intPageSize)
            };

        }

        [HttpGet("{id}")]
        public async Task<MessageModel<ZrzyProject>> Get(string id)
        {
            return new MessageModel<ZrzyProject>()
            {
                msg = "获取成功",
                success = true,
                response = await _zrzyProjectServices.QueryById(id)
            };
        }

        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] ZrzyProject request)
        {
            var data = new MessageModel<string>();

            if (request != null) 
            {
                request.CreateId = _user.ID;
                request.CreateBy = _user.Name;
                request.CreateTime = DateTime.Now;
                request.Enabled = true;
                request.IsDeleted = false;
            }

            var id = await _zrzyProjectServices.Add(request);
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] ZrzyProject request)
        {
            var data = new MessageModel<string>();

            if (request != null)
            {
                request.ModifyId = _user.ID;
                request.ModifyBy = _user.Name;
                request.ModifyTime = DateTime.Now;
            }

            data.success = await _zrzyProjectServices.Update(request);
            if (data.success)
            {
                data.msg = "更新成功";
                data.response = request?.Id.ObjToString();
            }

            return data;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<MessageModel<string>> DeleteSoft([FromBody] ZrzyProject request)
        {
            var data = new MessageModel<string>();
            if (request != null)
            {
                request.DeleteBy = _user.Name;
                request.DeleteTime = DateTime.Now;
                request.IsDeleted = true;
            }
            data.success = await _zrzyProjectServices.DeleteSoft(request);
            if (data.success)
            {
                data.msg = "删除成功";
                data.response = request?.Id.ObjToString(); ;
            }

            return data;
        }

        [HttpDelete("{id}")]
        public async Task<MessageModel<string>> Delete(string id)
        {
            var data = new MessageModel<string>();

            data.success = await _zrzyProjectServices.DeleteById(id);
            if (data.success)
            {
                data.msg = "删除成功";
                data.response = id;
            }

            return data;
        }


        /// <summary>
        /// 通过项目获取用户编号
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<int>>> GetUserIdsByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<int>>();

            var pus = await _zrzyProjectUserServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var userIds = (from item in pus
                                   select item.UserId.ObjToInt()).ToList();


            data.success = true;
            if (data.success)
            {
                data.response = userIds;
                data.msg = "获取成功";
            }

            return data;
        }


        /// <summary>
        /// 通过项目获取用户
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<sysUserInfo>>> GetUsersByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<sysUserInfo>>();

            var pus = await _zrzyProjectUserServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var userIds = (from item in pus
                           select item.UserId.ObjToInt()).ToList();

            object[] param = Array.ConvertAll<int, object>(userIds.ToArray(), new Converter<int, object>(Int2Obj));
            var result = await _sysUserInfoServices.QueryByIDs(param);

            data.success = true;
            if (data.success)
            {
                data.response = result;
                data.msg = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 保存项目用户分配
        /// </summary>
        /// <param name="assignView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> ProjectUsersAssign([FromBody] ProjectUsersView assignView)
        {
            var data = new MessageModel<string>();


            if (assignView.pid > 0)
            {
                data.success = true;

                var projectUsers = await _zrzyProjectUserServices.Query(d => d.ProjectId == assignView.pid);
                //TODO
                var remove = projectUsers.Where(d => !assignView.uids.Contains(d.UserId.ObjToInt())).Select(c => (object)c.Id);
                data.success &= remove.Any() ? await _zrzyProjectUserServices.DeleteByIds(remove.ToArray()) : true;

                foreach (var item in assignView.uids)
                {
                    var rmpitem = projectUsers.Where(d => d.UserId == item);
                    if (!rmpitem.Any())
                    {
                        ZrzyProjectUser projectUser = new ZrzyProjectUser();
                        projectUser.IsDeleted = false;
                        projectUser.ProjectId = assignView.pid;
                        projectUser.UserId = item;
                        projectUser.Enabled = true;
                        projectUser.CreateId = _user.ID;
                        projectUser.CreateBy = _user.Name;
                        projectUser.CreateTime = DateTime.Now;

                        data.success &= (await _zrzyProjectUserServices.Add(projectUser)) > 0;

                    }
                }

                if (data.success)
                {
                    _requirement.Permissions.Clear();
                    data.response = "";
                    data.msg = "保存成功";
                }

            }


            return data;
        }


        /// <summary>
        /// 通过项目获取文件编号
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<int>>> GetFileIdsByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<int>>();

            var pus = await _zrzyProjectFileServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var userIds = (from item in pus
                           select item.FileId.ObjToInt()).ToList();
            data.success = true;
            if (data.success)
            {
                data.response = userIds;
                data.msg = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 通过项目获取文件列表
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<ZrzyFile>>> GetFilesByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<ZrzyFile>>();

            var pus = await _zrzyProjectFileServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var fileIds = (from item in pus
                           select item.FileId.ObjToInt()).ToList();

            object[] param = Array.ConvertAll<int, object>(fileIds.ToArray(), new Converter<int, object>(Int2Obj));
            var result =  await _zrzyFileServices.QueryByIDs(param);

            data.success = true;
            if (data.success)
            {
                data.response = result;
                data.msg = "获取成功";
            }

            return data;
        }

        private static object Int2Obj(int param)
        {
            return (object)param;
        }
        /// <summary>
        /// 保存项目文件分配
        /// </summary>
        /// <param name="assignView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> ProjectFilesAssign([FromBody] ProjectFilesView assignView)
        {
            var data = new MessageModel<string>();


            if (assignView.pid > 0)
            {
                data.success = true;

                var projectFiles = await _zrzyProjectFileServices.Query(d => d.ProjectId == assignView.pid);
                //TODO
                var remove = projectFiles.Where(d => !assignView.fids.Contains(d.FileId.ObjToInt())).Select(c => (object)c.Id);
                data.success &= remove.Any() ? await _zrzyProjectFileServices.DeleteByIds(remove.ToArray()) : true;

                foreach (var item in assignView.fids)
                {
                    var rmpitem = projectFiles.Where(d => d.FileId == item);
                    if (!rmpitem.Any())
                    {
                        ZrzyProjectFile projectFile = new ZrzyProjectFile();
                        projectFile.IsDeleted = false;
                        projectFile.ProjectId = assignView.pid;
                        projectFile.FileId = item;
                        projectFile.Enabled = true;
                        projectFile.CreateId = _user.ID;
                        projectFile.CreateBy = _user.Name;
                        projectFile.CreateTime = DateTime.Now;

                        data.success &= (await _zrzyProjectFileServices.Add(projectFile)) > 0;

                    }
                }

                if (data.success)
                {
                    _requirement.Permissions.Clear();
                    data.response = "";
                    data.msg = "保存成功";
                }

            }
            return data;
        }


        /// <summary>
        /// 通过项目获取地图服务编号
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<int>>> GetMapServerIdsByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<int>>();

            var pus = await _zrzyProjectMapServerServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var userIds = (from item in pus
                           select item.MapServerId.ObjToInt()).ToList();


            data.success = true;
            if (data.success)
            {
                data.response = userIds;
                data.msg = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 通过项目获取地图服务列表
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<ZrzyMapServer>>> GetMapServersByProjectId(int pid = 0)
        {
            var data = new MessageModel<List<ZrzyMapServer>>();

            var pus = await _zrzyProjectMapServerServices.Query(d => d.IsDeleted == false && d.ProjectId == pid);
            var ids = (from item in pus
                           select item.MapServerId.ObjToInt()).ToList();
            object[] param = Array.ConvertAll<int, object>(ids.ToArray(), new Converter<int, object>(Int2Obj));
            var result = await _zrzyMapServerServices.QueryByIDs(param);

            data.success = true;
            if (data.success)
            {
                data.response = result;
                data.msg = "获取成功";
            }

            return data;
        }

        /// <summary>
        /// 保存项目地图服务分配
        /// </summary>
        /// <param name="assignView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> ProjectMapServersAssign([FromBody] ProjectMapserversView assignView)
        {
            var data = new MessageModel<string>();


            if (assignView.pid > 0)
            {
                data.success = true;

                var projectMapServers = await _zrzyProjectMapServerServices.Query(d => d.ProjectId == assignView.pid);
                //TODO
                var remove = projectMapServers.Where(d => !assignView.mids.Contains(d.MapServerId.ObjToInt())).Select(c => (object)c.Id);
                data.success &= remove.Any() ? await _zrzyProjectFileServices.DeleteByIds(remove.ToArray()) : true;

                foreach (var item in assignView.mids)
                {
                    var rmpitem = projectMapServers.Where(d => d.MapServerId == item);
                    if (!rmpitem.Any())
                    {
                        ZrzyProjectMapServer projectFile = new ZrzyProjectMapServer();
                        projectFile.IsDeleted = false;
                        projectFile.ProjectId = assignView.pid;
                        projectFile.MapServerId = item;
                        projectFile.Enabled = true;
                        projectFile.CreateId = _user.ID;
                        projectFile.CreateBy = _user.Name;
                        projectFile.CreateTime = DateTime.Now;

                        data.success &= (await _zrzyProjectMapServerServices.Add(projectFile)) > 0;

                    }
                }

                if (data.success)
                {
                    _requirement.Permissions.Clear();
                    data.response = "";
                    data.msg = "保存成功";
                }

            }
            return data;
        }
    }

    public class ProjectUsersView
    {
        public List<int> uids { get; set; }
        public int pid { get; set; }
    }

    public class ProjectFilesView
    {
        public List<int> fids { get; set; }
        public int pid { get; set; }
    }
    public class ProjectMapserversView
    {
        public List<int> mids { get; set; }
        public int pid { get; set; }
    }

}