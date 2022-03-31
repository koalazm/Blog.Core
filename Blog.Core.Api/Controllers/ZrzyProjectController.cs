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
        private readonly IUser _user;
        private readonly PermissionRequirement _requirement;

        public ZrzyProjectController(IZrzyProjectServices zrzyProjectServices,
            IZrzyProjectUserServices zrzyProjectUserServices,
            IUser user, 
            PermissionRequirement requirement)
        {
            _zrzyProjectServices = zrzyProjectServices;
            _zrzyProjectUserServices = zrzyProjectUserServices;
            _user = user;
            _requirement= requirement ;
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
        /// 通过项目获取用户
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
        /// 保存菜单权限分配
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

    }

    public class ProjectUsersView
    {
        public List<int> uids { get; set; }
        public int pid { get; set; }
    }
}