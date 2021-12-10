#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：SettingViewModel
// 创 建 者：杨程
// 创建时间：2021/12/10 10:51:36
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
    public class SettingViewModel:ViewModelBase
    {
        private IDialogMessage Dialog { get; set; } 
        public SettingViewModel(IDataContext dc,IAppConfig config,IDialogMessage dialog):base(dc,config)
        {
            Dialog = dialog;
            //构造函数
            Title = "设置";
        }

        public override void InitData()
        {
            Systems = new ObservableCollection<SystemClass>();
            NewSystemClass = new SystemClass();

            Modules = new ObservableCollection<ModuleClass>();
            NewModuleClass=new ModuleClass();
            GetSystemData();
            GetModuleData();
        }

        #region 属性
        public ObservableCollection<SystemClass> Systems { get; set; }

        public ObservableCollection<ModuleClass> Modules { get; set; }

        #region 新增系统分类的model
        private SystemClass _NewSystemClass;
        /// <summary>
        /// 新增系统分类的model
        /// </summary>
        public SystemClass NewSystemClass
        {
            get { return _NewSystemClass; }
            set { _NewSystemClass = value; DoNotify(); }
        }
        #endregion

        #region 新增功能模块分类的model
        private ModuleClass _NewModuleClass;
        /// <summary>
        /// 新增功能模块分类的model
        /// </summary>
        public ModuleClass NewModuleClass
        {
            get { return _NewModuleClass; }
            set { _NewModuleClass = value; DoNotify(); }
        }

        private SystemClass _ModuleSelectSystem;

        public SystemClass ModuleSelectSystem
        {
            get { return _ModuleSelectSystem; }
            set { _ModuleSelectSystem = value; DoNotify(); }
        }

        #endregion

        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        private void GetSystemData()
        {
            var systems=DC.Set<SystemClass>().ToList();
            Systems.Clear();

            foreach (var system in systems)
            {
                Systems.Add(system);
            }

        }

        private void GetModuleData()
        {
            var modules = DC.Set<ModuleClass>().ToList();
            Modules.Clear();

            foreach (var module in modules)
            {
                Modules.Add(module);
            }

        }
        #endregion

        #region 命令

        public RelayCommand AddNewSystemClassCommand => new RelayCommand(() => 
        {
            NewSystemClass=new SystemClass() { IsEnable=true,CreateTime=DateTime.Now};
        });

        public RelayCommand InsertSystemClassCommand => new RelayCommand(() => 
        {
            if (!string.IsNullOrEmpty(NewSystemClass.SystemName))
            {
                using(var trans = DC.Database.BeginTransaction())
                {
                    try
                    {
                        DC.AddEntity(NewSystemClass);
                        DC.SaveChanges();

                        trans.Commit();

                        GetSystemData();
                    }
                    catch (Exception ex)
                    {
                        Dialog.ShowPopupWindow(ex.Message,(Window)View, Vampirewal.Core.WpfTheme.WindowStyle.MessageType.Error);
                        trans.Rollback();
                    }
                }
            }
        });


        public RelayCommand AddNewModuleClassCommand => new RelayCommand(() => 
        {
            NewModuleClass = new ModuleClass();
        });

        public RelayCommand InsertModuleClassCommand => new RelayCommand(() => 
        {
            if (ModuleSelectSystem != null)
            {
                NewModuleClass.SystemClassId = ModuleSelectSystem.ID;
                NewModuleClass.SystemName = ModuleSelectSystem.SystemName;
                NewModuleClass.IsEnable = true;
                NewModuleClass.CreateTime = DateTime.Now;

                if (!string.IsNullOrEmpty(NewModuleClass.ModuleName))
                {
                    using (var trans = DC.Database.BeginTransaction())
                    {
                        try
                        {
                            DC.AddEntity(NewModuleClass);
                            DC.SaveChanges();

                            trans.Commit();

                            GetModuleData();
                        }
                        catch (Exception ex)
                        {
                            Dialog.ShowPopupWindow(ex.Message, (Window)View, Vampirewal.Core.WpfTheme.WindowStyle.MessageType.Error);
                            trans.Rollback();
                        }
                    }
                }
            }

            
        });
        #endregion
    }
}
