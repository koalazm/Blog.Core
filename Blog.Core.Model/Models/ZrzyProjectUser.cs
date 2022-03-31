using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace Blog.Core.Model.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("zrzyprojectuser", "server=localhost;Database=OneMapDB1;Uid=root;Pwd=root;Port=3306;Allow User Variables=True; ")]
    public class ZrzyProjectUser
    {
        public ZrzyProjectUser()
        {
        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? CreateId { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public string DeleteBy { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public string ModifyBy { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public int? ModifyId { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public int ProjectId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public sysUserInfo User { get; set; }
        [SugarColumn(IsIgnore = true)]
        public ZrzyProject Project { get; set; }
    }
}