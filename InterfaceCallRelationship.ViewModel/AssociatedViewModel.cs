#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：AssociatedViewModel
// 创 建 者：杨程
// 创建时间：2021/12/15 15:32:23
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using InterfaceCallRelationship.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vampirewal.Core.Interface;
using Vampirewal.Core.SimpleMVVM;

namespace InterfaceCallRelationship.ViewModel
{
    /// <summary>
    /// 关联方法ViewModel
    /// </summary>
    public class AssociatedViewModel:ViewModelBase
    {
        private IDialogMessage Dialog { get; set; }
        public AssociatedViewModel(IDataContext dc,IDialogMessage dialog):base(dc)
        {
            Dialog = dialog;
            //构造函数
        }

        private List<string> ResultIds { get; set; }
        public override object GetResult()
        {
            return ResultIds;
        }

        public override void PassData(object obj)
        {
            Source = obj as MethodClass;
            GetData();
        }

        public override void InitData()
        {
            Functions = new ObservableCollection<MethodClass>();
            ResultIds = new List<string>();
            
        }

        #region 属性
        public ObservableCollection<MethodClass> Functions { get; set; }

        private MethodClass Source { get; set; }
        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        /// <summary>
        /// 获取数据
        /// </summary>
        private void GetData()
        {
            //获取的数据中，排除掉自己本身
            var current=DC.Set<MethodClass>().Where(w=>w.ID!=Source.ID).ToList();
            Functions.Clear();

            foreach (var item in current)
            {
                
                Functions.Add(item);
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand SaveCommand => new RelayCommand(() => 
        {
            //获取到已选择的方法
            if (Functions.Any(a=>a.Checked))
            {
                //挨个写入要返回上一个页面的列表内
                foreach (var item in Functions.Where(a => a.Checked).ToList())
                {
                    ResultIds.Add(item.ID);
                }

                ((Window)View).Close();
            }
            else
            {
                Dialog.ShowPopupWindow("未选择任何功能进行关联！", (Window)View, Vampirewal.Core.WpfTheme.WindowStyle.MessageType.Error);
            }
        });

        #endregion
    }
}
