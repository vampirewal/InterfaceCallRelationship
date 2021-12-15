#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：MethodNode
// 创 建 者：杨程
// 创建时间：2021/12/14 14:46:44
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vampirewal.Core.SimpleMVVM;

namespace InterfaceCallRelationship.Model
{
    /// <summary>
    /// 方法节点类，用于前端展示
    /// </summary>
    public class MethodNode : NotifyBase
    {
        public string ParentId { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 功能模块名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [Required]
        public string MethodName { get; set; }

        public bool IsTopNode { get; set; } = false;

        public List<string> SourceList { get; set; } = new List<string>();
        /// <summary>
        /// 是否有引用
        /// </summary>
        public bool IsSource { get { return SourceList.Count > 0; } }


        public List<string> TargetList { get; set; } = new List<string>();
        /// <summary>
        /// 是否有被引用
        /// </summary>
        public bool IsReferenced { get { return TargetList.Count > 0; } }

    }
}
