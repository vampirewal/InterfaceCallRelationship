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
using Org.BouncyCastle.Asn1.X509;
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
    public class MainViewModel : ViewModelBase
    {
        private IDialogMessage Dialog { get; set; }
        public MainViewModel(IAppConfig config, IDataContext dc, IDialogMessage dialog) : base(dc, config)
        {
            Dialog = dialog;
            //构造函数
            Title = Config.AppChineseName;
        }

        public override void InitData()
        {
            Functions = new ObservableCollection<FunctionClass>();

            GetData();

            Methods = new ObservableCollection<MethodNode>();
        }

        #region 属性
        public ObservableCollection<FunctionClass> Functions { get; set; }

        public ObservableCollection<MethodNode> Methods { get; set; }
        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        private void GetData()
        {
            var cur = DC.Set<FunctionClass>().ToList();

            Functions.Clear();
            foreach (var item in cur)
            {
                var curMethods = DC.Set<MethodClass>().Where(w => w.FunctionClassId == item.ID).ToList();
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
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.SettingView),
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
            bool IsOk=Convert.ToBoolean( Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.AddNewFunctionView),
                WindowWidth = 680,
                WindowHeight = 450,
                IconStr = "",
                IsShowMaxButton = false,
                IsShowMinButton = false
            }));

            if (IsOk)
            {
                GetData();
            }
        });

        public RelayCommand<FunctionClass> EditDataCommand => new RelayCommand<FunctionClass>((f) => 
        {
            bool IsOk = Convert.ToBoolean(Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.AddNewFunctionView),
                PassData = f,
                WindowWidth = 680,
                WindowHeight = 450,
                IconStr = "",
                IsShowMaxButton = false,
                IsShowMinButton = false
            }));

            if (IsOk)
            {
                GetData();
            }
        });

        public RelayCommand<MethodClass> AssociatedCommand => new RelayCommand<MethodClass>((m) =>
        {
            List<string>? SelectedItems = Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.AssociatedView),
                WindowWidth = 680,
                WindowHeight = 450,
                IconStr = "",
                IsShowMaxButton = false,
                IsShowMinButton = false,
                PassData = m
            }) as List<string>;



            if (SelectedItems != null && SelectedItems.Count > 0)
            {


                using (var trans = DC.Database.BeginTransaction())
                {
                    try
                    {
                        var deletes = DC.Set<MethodClassReferenceRelationship>().Where(w => w.SourceId == m.ID).ToList();
                        foreach (var delete in deletes)
                        {
                            DC.DeleteEntity(delete);
                        }

                        foreach (var item in SelectedItems)
                        {
                            MethodClassReferenceRelationship referenceRelationship = new MethodClassReferenceRelationship()
                            {
                                SourceId = m.ID,
                                ReferenceId = item
                            };

                            DC.AddEntity(referenceRelationship);
                        }

                        DC.SaveChanges();

                        trans.Commit();


                    }
                    catch (Exception ex)
                    {
                        Dialog.ShowPopupWindow(ex.Message, (Window)View, Vampirewal.Core.WpfTheme.WindowStyle.MessageType.Error);
                        trans.Rollback();
                    }
                }
            }
        });


        public RelayCommand<MethodClass> LookReferenceCommnad => new RelayCommand<MethodClass>(LookReference);

        private void LookReference(MethodClass m)
        {
            Methods.Clear();

            var sources = DC.Set<MethodClassReferenceRelationship>().Where(w => w.SourceId == m.ID).Select(s => s.ReferenceId).ToList();
            var References = DC.Set<MethodClassReferenceRelationship>().Where(w => w.ReferenceId == m.ID).Select(s => s.SourceId).ToList();

            Methods.Add(new MethodNode()
            {
                Id = m.ID,
                FunctionName =$"({m.SystemClassName}-{m.ModuleClassName}){m.FunctionClassName}" ,
                MethodName = m.MethodName,
                IsTopNode = true,
                SourceList = References,
                TargetList = sources,
            });
        }


        public RelayCommand<MethodNode> ShowDownCommand => new RelayCommand<MethodNode>((m) =>
        {
            foreach (var item in m.TargetList)
            {
                if (Methods.Any(a=>a.Id==item))
                {
                    return;
                }

                var target = DC.Set<MethodClass>().Where(w => w.ID == item).FirstOrDefault();
                if (target != null)
                {
                    var sources = DC.Set<MethodClassReferenceRelationship>().Where(w => w.SourceId == target.ID&&w.ReferenceId!=m.Id).Select(s => s.ReferenceId).ToList();
                    var References = DC.Set<MethodClassReferenceRelationship>().Where(w => w.ReferenceId == target.ID && w.SourceId != m.Id).Select(s => s.SourceId).ToList();

                    Methods.Add(new MethodNode()
                    {
                        ParentId = m.Id,
                        Id = target.ID,
                        FunctionName = $"({target.SystemClassName}-{target.ModuleClassName}){target.FunctionClassName}",
                        MethodName = target.MethodName,
                        IsTopNode = false,
                        SourceList = References,
                        TargetList = sources,
                    });
                }

            }


        });

        public RelayCommand<MethodNode> ShowUpCommand => new RelayCommand<MethodNode>((m) =>
        {
            foreach (var item in m.SourceList)
            {
                if (Methods.Any(a => a.Id == item))
                {
                    return;
                }

                var source = DC.Set<MethodClass>().Where(w => w.ID == item).FirstOrDefault();
                if (source != null)
                {
                    var sources = DC.Set<MethodClassReferenceRelationship>().Where(w => w.SourceId == source.ID && w.ReferenceId != m.Id).Select(s => s.ReferenceId).ToList();
                    var References = DC.Set<MethodClassReferenceRelationship>().Where(w => w.ReferenceId == source.ID && w.SourceId != m.Id).Select(s => s.SourceId).ToList();

                    Methods.Add(new MethodNode()
                    {
                        ParentId = m.Id,
                        Id = source.ID,
                        FunctionName = source.FunctionClassName,
                        MethodName = source.MethodName,
                        IsTopNode = false,
                        SourceList = References,
                        TargetList = sources,
                    });
                }
            }
        });
        #endregion
    }
}
