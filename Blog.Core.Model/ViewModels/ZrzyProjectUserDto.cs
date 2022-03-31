using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Model.ViewModels
{
    public class ZrzyProjectUserDto
    {
        //UserName = u.uRealName,
        //ProjectName = p.ProjectName,
        //UserId = up.UserId,
        //ProjectId = up.Id,
        //UserProjectId = up.Id

            public string UserName { get; set; }
            public string ProjectName { get; set; }
            public int UserId { get; set; }
            public int ProjectId { get; set; }
            public int UserProjectId { get; set; }
    }
}
