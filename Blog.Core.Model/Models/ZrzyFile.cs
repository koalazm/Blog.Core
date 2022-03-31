using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace Blog.Core.Model.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("zrzyfile", "server=localhost;Database=OneMapDB;Uid=root;Pwd=root;Port=3306;Allow User Variables=True;")]
    public class ZrzyFile
    {
        public ZrzyFile()
        {
        }
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
        public string FileName { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public double FileSize { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        public string GuidName { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
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
        /// Nullable:False
        /// </summary>
        public int OrderSort { get; set; }
    }
}