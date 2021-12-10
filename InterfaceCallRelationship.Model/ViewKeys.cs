#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：ViewKeys
// 创 建 者：杨程
// 创建时间：2021/12/10 9:40:34
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceCallRelationship.Model
{
    public class ViewKeys
    {
        private static string BaseViewKey = "InterfaceCallRelationship";

        /// <summary>
        /// 主窗体
        /// </summary>
        public static string MainView = $"{BaseViewKey}.MainWindow";

        /// <summary>
        /// 设置窗体
        /// </summary>
        public static string SettingView = $"{BaseViewKey}.SettingView";

        /// <summary>
        /// 新增功能窗体
        /// </summary>
        public static string AddNewFunctionView = $"{BaseViewKey}.AddNewFunctionView";
    }
}
