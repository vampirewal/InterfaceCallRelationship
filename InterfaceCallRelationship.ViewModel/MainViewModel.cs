#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：MainViewModel
// 创 建 者：杨程
// 创建时间：2021/12/10 9:50:40
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vampirewal.Core.Interface;
using Vampirewal.Core.SimpleMVVM;

namespace InterfaceCallRelationship.ViewModel
{
    public class MainViewModel:ViewModelBase
    {
        private IDialogMessage Dialog { get; set; }
        public MainViewModel(IAppConfig config,IDataContext dc,IDialogMessage dialog):base(dc,config)
        {
            Dialog = dialog;
            //构造函数
            Title = Config.AppChineseName;
        }

        public override void InitData()
        {
            Functions = new ObservableCollection<FunctionClass>();

            GetData();
        }
        #region 属性
        public ObservableCollection<FunctionClass> Functions { get; set; }
        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        private void GetData()
        {
            var cur=DC.Set<FunctionClass>().ToList();

            Functions.Clear();
            foreach (var item in cur)
            {
                var curMethods = DC.Set<MethodClass>().Where(w => w.FunctionClassId==item.ID).ToList();
                item.methods.Clear();
                foreach (var method in curMethods)
                {
                    item.methods.Add(method);
                }

                Functions.Add(item);
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 打开设置窗体
        /// </summary>
        public RelayCommand OpenSettingWindowCommand => new RelayCommand(() => 
        {
            Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView=Messenger.Default.Send<FrameworkElement>("GetView",ViewKeys.SettingView),
                WindowWidth = 700,
                WindowHeight = 430,
                IconStr = "",
                IsShowMaxButton = false,
                IsShowMinButton = false
            });
        });

        /// <summary>
        /// 刷新数据命令
        /// </summary>
        public RelayCommand RefreshDataCommand => new RelayCommand(() => 
        {
            GetData();
        });


        public RelayCommand AddNewDataCommand => new RelayCommand(() => 
        {
            Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.AddNewFunctionView),
                WindowWidth = 680,
                WindowHeight = 450,
                IconStr = "",
                IsShowMaxButton = false,
                IsShowMinButton = false
            });
        });
        #endregion
    }
}
