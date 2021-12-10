#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：FunctionClass
// 创 建 者：杨程
// 创建时间：2021/12/10 15:40:01
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vampirewal.Core.Models;

namespace InterfaceCallRelationship.Model
{
    [Table("Function")]
    public class FunctionClass:TopBaseModel
    {
        public FunctionClass()
        {
            methods = new ObservableCollection<MethodClass>();
        }

        /// <summary>
        /// 系统ID
        /// </summary>
        public string SystemClassId { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemClassName { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleClassId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleClassName { get; set; }

        /// <summary>
        /// 功能模块名称
        /// </summary>
        public string FuncitonName { get; set; }

        /// <summary>
        /// 名下的方法
        /// </summary>
        [NotMapped]
        public ObservableCollection<MethodClass> methods { get; set; }
    }

    [Table("Method")]
    public class MethodClass:TopBaseModel
    {
        /// <summary>
        /// 功能模块ID
        /// </summary>
        public string FunctionClassId { get; set; }

        /// <summary>
        /// 功能模块名称
        /// </summary>
        public string FunctionClassName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 是否公有方法
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// 引用数量
        /// </summary>
        public int ReferenceCount { get; set; }
    }
}
