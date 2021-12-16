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
    /// <summary>
    /// 主窗体ViewModel
    /// </summary>
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
        /// <summary>
        /// 功能模块列表，用于界面左侧
        /// </summary>
        public ObservableCollection<FunctionClass> Functions { get; set; }

        /// <summary>
        /// 方法列表，用于功能模块中查看方法
        /// </summary>
        public ObservableCollection<MethodNode> Methods { get; set; }
        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        /// <summary>
        /// 初始获取数据
        /// </summary>
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
            /*
             * 此处的Dialog.OpenDialogWindow的详细用法，可以看我的博客中介绍 https://blog.csdn.net/weixin_42806176/article/details/120842040
             */

            Dialog.OpenDialogWindow(new Vampirewal.Core.WpfTheme.WindowStyle.DialogWindowSetting()
            {
                UiView = Messenger.Default.Send<FrameworkElement>("GetView", ViewKeys.SettingView),
                WindowWidth = 700,
                WindowHeight = 430,
                IconStr = "",//如果其他地方也需要使用这个IconStr代码的话，请按照我这样写，直接赋值“”,因为有问题，但是我没时间去测试和修改。。。
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

        /// <summary>
        /// 新增功能模块命令
        /// </summary>
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

        /// <summary>
        /// 编辑功能模块命令
        /// </summary>
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

        /// <summary>
        /// 建立方法之间引用关系的命令
        /// </summary>
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

        /// <summary>
        /// 查看引用关系命令
        /// </summary>
        /// <param name="m"></param>
        private void LookReference(MethodClass m)
        {
            /*
             * 触发这个命令的是我定义为最顶层的节点，所以需要清空界面中的node
             * 然后根据这个最顶层的节点，重新开始绘制
             */
            Methods.Clear();

            //获取引用的目标Id
            var sources = DC.Set<MethodClassReferenceRelationship>().Where(w => w.SourceId == m.ID).Select(s => s.ReferenceId).ToList();
            //获取引用自己的父级ID
            var References = DC.Set<MethodClassReferenceRelationship>().Where(w => w.ReferenceId == m.ID).Select(s => s.SourceId).ToList();

            //这个就很简单了，简单赋值添加进界面的数据源中即可
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

        /// <summary>
        /// 向下展开命令
        /// </summary>
        public RelayCommand<MethodNode> ShowDownCommand => new RelayCommand<MethodNode>((m) =>
        {
            //
            foreach (var item in m.TargetList)
            {
                //这里加了1个判断，为了避免重复点击同一个向下展开按钮，在界面上重复创建node
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

        /// <summary>
        /// 向上展开命令
        /// </summary>
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
