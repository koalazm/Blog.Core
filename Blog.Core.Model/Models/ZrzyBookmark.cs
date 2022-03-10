using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace Blog.Core.Model.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable( "zrzybookmark", "server=localhost;Database=OneMapDB;Uid=root;Pwd=root;Port=3306;Allow User Variables=True;")]
    public class ZrzyBookmark
    {
           public ZrzyBookmark()
           {
           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>
           public string BookmarkName { get; set; }
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
           public string Description { get; set; }
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
           public int? ExtentId { get; set; }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int Id { get; set; }
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
           public int OrderSort { get; set; }
    }
}