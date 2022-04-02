using Blog.Core.IServices;
using Blog.Core.Model;
using Blog.Core.Model.Models;
using Blog.Core.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.Core.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ZrzyFileController : ControllerBase
    {
        /// <summary>
        /// 服务器接口，因为是模板生成，所以首字母是大写的，自己可以重构下
        /// </summary>
        private readonly IZrzyFileServices _zrzyFileServices;

        public ZrzyFileController(IZrzyFileServices fileServices)
        {
            _zrzyFileServices = fileServices;
        }


        [HttpGet("{id}")]
        public async Task<MessageModel<ZrzyFile>> Get(int id)
        {
            return new MessageModel<ZrzyFile>()
            {
                msg = "获取成功",
                success = true,
                response = await _zrzyFileServices.QueryById(id)
            };
        }


        [HttpGet]
        public async Task<MessageModel<List<ZrzyFileViewModel>>> GetFileList()
        {
            List<ZrzyFile> zrzyFiles = await _zrzyFileServices.Query();

            if (zrzyFiles == null || zrzyFiles.Count <= 0)
            {
                return new MessageModel<List<ZrzyFileViewModel>>()
                {
                    msg = "获取成功",
                    success = true,
                    response = null
                };
            }

            List<ZrzyFileViewModel> zrzyFileViewModels = new();

            foreach (var item in zrzyFiles)
            {
                zrzyFileViewModels.Add(new ZrzyFileViewModel
                {
                    Id = item.Id,
                    Enabled = item.Enabled,
                    FileName = item.FileName,
                    FileSize = item.FileSize,
                    FileType = item.FileType,
                    FileUrl = String.Concat("/api/ZrzyFile/DownFile?filename=", item.GuidName),
                    GuidName = item.GuidName,
                    IsDeleted = item.IsDeleted,
                    OrderSort = item.OrderSort

                });
            }

            return new MessageModel<List<ZrzyFileViewModel>>()
            {
                msg = "获取成功",
                success = true,
                response = zrzyFileViewModels
            };

        }

        [HttpGet]
        public async Task<MessageModel<PageModel<ZrzyFileViewModel>>> GetPagedFileList(int page = 1, string key = "", int intPageSize = 50)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            Expression<Func<ZrzyFile, bool>> whereExpression = t => t.FileName.Contains(key);
            PageModel<ZrzyFile> zrzyFiles = await _zrzyFileServices.QueryPage(whereExpression, page, intPageSize);
            PageModel<ZrzyFileViewModel> zrzyFileViewModels = new PageModel<ZrzyFileViewModel>()
            {
                dataCount = zrzyFiles.dataCount,
                page = zrzyFiles.page,
                //pageCount = zrzyFiles.pageCount,
                PageSize = zrzyFiles.PageSize
            };
            zrzyFileViewModels.data = new System.Collections.Generic.List<ZrzyFileViewModel>();


            foreach (var item in zrzyFiles.data)
            {
                zrzyFileViewModels.data.Add(new ZrzyFileViewModel
                {
                    Id = item.Id,
                    Enabled = item.Enabled,
                    FileName = item.FileName,
                    FileSize = item.FileSize,
                    FileType = item.FileType,
                    FileUrl = String.Concat("/api/ZrzyFile/DownFile?filename=", item.GuidName),
                    GuidName = item.GuidName,
                    IsDeleted = item.IsDeleted,
                    OrderSort = item.OrderSort

                });
            }

            return new MessageModel<PageModel<ZrzyFileViewModel>>()
            {
                msg = "获取成功",
                success = true,
                response = zrzyFileViewModels
            };

        }

        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] ZrzyFile request)
        {
            var data = new MessageModel<string>();

            var id = await _zrzyFileServices.Add(request);
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] ZrzyFile request)
        {
            var data = new MessageModel<string>();
            data.success = await _zrzyFileServices.Update(request);
            if (data.success)
            {
                data.msg = "更新成功";
                data.response = request?.Id.ObjToString();
            }

            return data;
        }

        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            data.success = await _zrzyFileServices.DeleteById(id);
            if (data.success)
            {
                data.msg = "删除成功";
                data.response = id.ToString();
            }

            return data;
        }

        /// <summary>
        /// 上传单个文件，缓存式文件上传
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> UploadFile(IFormFile file, [FromServices] IWebHostEnvironment environment)
        {
            string foldername = "files";
            var data = new MessageModel<string>();

            var allowType = new string[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

            if (file == null) { data.msg = "请选择上传的文件。"; return data; }

            if (!allowType.Contains(file.ContentType))
            {
                data.msg = "文件格式错误，仅支持pdf、word文档上传";
                return data;
            }

            if (file.Length > 1024 * 1024 * 50)
            {
                data.msg = "文件过大";
                return data;
            }

            Guid guid = Guid.NewGuid();

            string newFileName = guid.ToString().Replace("{", "").Replace("}", "") + Path.GetExtension(file.FileName);
            double fileSize = file.Length / 1048576;
            using (var stream = file.OpenReadStream())
            {
                //var trustedFileNameForFileStorage = Path.GetRandomFileName();
                string dirName = Path.Combine(environment.WebRootPath, foldername);

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                await WriteFileAsync(stream, Path.Combine(dirName, newFileName));
            }

            ZrzyFile zrzyFile = new ZrzyFile
            {
                FileName = file.FileName,
                FileSize = Math.Round(fileSize, 2),
                GuidName = newFileName,
                FileType = file.ContentType,
                OrderSort = 1,
                Enabled = true,
                IsDeleted = false
            };

            var id = await _zrzyFileServices.Add(zrzyFile);
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "上传成功";
                data.status = 200;
                data.success = true;
            }
            return data;
        }

        /// <summary>
        /// 写文件导到磁盘
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="path">文件保存路径</param>
        /// <returns></returns>
        public static async Task<int> WriteFileAsync(System.IO.Stream stream, string path)
        {
            const int FILE_WRITE_SIZE = 84975;//写出缓冲区大小
            int writeCount = 0;
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, FILE_WRITE_SIZE, true))
            {
                byte[] byteArr = new byte[FILE_WRITE_SIZE];
                int readCount = 0;
                while ((readCount = await stream.ReadAsync(byteArr, 0, byteArr.Length)) > 0)
                {
                    await fileStream.WriteAsync(byteArr, 0, readCount);
                    writeCount += readCount;
                }
            }
            return writeCount;
        }


        /// <summary>
        /// 上传多文件，可以使用 postman 测试，
        /// 如果是单文件，可以 参数写 IFormFile file1
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("/files/Upload/File2")]
        public async Task<MessageModel<string>> UpLoadFile2([FromServices] IWebHostEnvironment environment)
        {
            var data = new MessageModel<string>();
            string path = string.Empty;
            string foldername = "files";
            IFormFileCollection files = null;


            // 获取提交的文件
            files = Request.Form.Files;
            // 获取附带的数据
            var max_ver = Request.Form["max_ver"].ObjToString();


            if (files == null || !files.Any()) { data.msg = "请选择上传的文件。"; return data; }
            //格式限制
            var allowType = new string[] { "application/pdf" };

            string folderpath = Path.Combine(environment.WebRootPath, foldername);
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 10)
                {
                    //foreach (var file in files)
                    var file = files.FirstOrDefault();
                    string strpath = Path.Combine(foldername, Path.GetFileName(file.FileName));
                    path = Path.Combine(environment.WebRootPath, strpath);

                    using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(stream);
                    }

                    data = new MessageModel<string>()
                    {
                        response = strpath,
                        msg = "上传成功",
                        success = true,
                    };
                    return data;
                }
                else
                {
                    data.msg = "文件过大";
                    return data;
                }
            }
            else

            {
                data.msg = "文件格式错误";
                return data;
            }
        }

        // GET: api/Download
        /// <summary>
        /// 下载文件（支持中文字符）
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public FileStreamResult DownFile([FromServices] IWebHostEnvironment environment, string filename)
        {
            string foldername = "files";
            string filepath = Path.Combine(environment.WebRootPath, foldername, filename);
            if (!System.IO.File.Exists(filepath))
            {
                return null;
            }
            var stream = System.IO.File.OpenRead(filepath);
            string fileExt = ".pdf";  // 这里可以写一个获取文件扩展名的方法，获取扩展名
            //获取文件的ContentType
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            var memi = provider.Mappings[fileExt];
            var fileName = Path.GetFileName(filepath);

            return File(stream, memi, fileName);
        }

        // GET: api/GetFilePath
        /// <summary>
        /// 获取PDF文件下载路径
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<MessageModel<string>> GetFilePath([FromServices] IWebHostEnvironment environment, string filename)
        {
            string urlpath = String.Concat("/api/ZrzyFile/DownFile?filename=", filename);


            return Task.FromResult(new MessageModel<string>()
            {
                msg = "获取成功",
                success = true,
                response = urlpath
            });
        }
    }
}