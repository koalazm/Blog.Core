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
    [Authorize(Permissions.Name)]
     public class ZrzyBookmarkController : ControllerBase
    {
            /// <summary>
            /// 服务器接口，因为是模板生成，所以首字母是大写的，自己可以重构下
            /// </summary>
            private readonly IZrzyBookmarkServices _bookmarkServices;
    
            public ZrzyBookmarkController(IZrzyBookmarkServices bookmarkServices)
            {
                _bookmarkServices = bookmarkServices;
            }
    
            [HttpGet]
            public async Task<MessageModel<PageModel<ZrzyBookmark>>> Get(int page = 1, string key = "",int intPageSize = 50)
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                {
                    key = "";
                }
    
                Expression<Func<ZrzyBookmark, bool>> whereExpression = a => true;
    
                return new MessageModel<PageModel<ZrzyBookmark>>()
                {
                    msg = "获取成功",
                    success = true,
                    response = await _bookmarkServices.QueryPage(whereExpression, page, intPageSize)
                };

            }

            [HttpGet("{id}")]
            public async Task<MessageModel<ZrzyBookmark>> Get(string id)
            {
                return new MessageModel<ZrzyBookmark>()
                {
                    msg = "获取成功",
                    success = true,
                    response = await _bookmarkServices.QueryById(id)
                };
            }

            [HttpPost]
            public async Task<MessageModel<string>> Post([FromBody] ZrzyBookmark request)
            {
                var data = new MessageModel<string>();

                var id = await _bookmarkServices.Add(request);
                if (data.success)
                {
                    data.response = id.ObjToString();
                    data.msg = "添加成功";
                } 

                return data;
            }

            [HttpPut]
            public async Task<MessageModel<string>> Put([FromBody] ZrzyBookmark request)
            {
                var data = new MessageModel<string>();
                data.success = await _bookmarkServices.Update(request);
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = request?.Id.ObjToString();
                }

                return data;
            }

            [HttpDelete("{id}")]
            public async Task<MessageModel<string>> Delete(string id)
            {
                var data = new MessageModel<string>();
                data.success = await _bookmarkServices.DeleteById(id);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = id;
                }

                return data;
            }
    }
}