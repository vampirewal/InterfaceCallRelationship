#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：ModuleClass
// 创 建 者：杨程
// 创建时间：2021/12/10 15:01:52
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vampirewal.Core.Models;

namespace InterfaceCallRelationship.Model
{
    [Table("Module")]
    public class ModuleClass:TopBaseModel
    {
        private string _ModuleName;

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName
        {
            get { return _ModuleName; }
            set { _ModuleName = value;DoNotify(); }
        }

        private string _SystemClassId;

        public string SystemClassId
        {
            get { return _SystemClassId; }
            set { _SystemClassId = value; DoNotify(); }
        }

        private string _SystemName;

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName
        {
            get { return _SystemName; }
            set { _SystemName = value; DoNotify(); }
        }

        private bool _IsEnable;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get { return _IsEnable; }
            set { _IsEnable = value; }
        }


    }
}
