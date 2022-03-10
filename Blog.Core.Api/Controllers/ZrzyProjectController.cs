using Blog.Core.Common.HttpContextUser;
using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        private readonly IUser _user;

        public ZrzyProjectController(IZrzyProjectServices zrzyProjectServices, IUser user)
        {
            _zrzyProjectServices = zrzyProjectServices;
            _user = user;
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
    }
}