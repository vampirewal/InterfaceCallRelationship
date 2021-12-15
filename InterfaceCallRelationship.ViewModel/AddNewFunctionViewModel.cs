#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：AddNewFunctionViewModel
// 创 建 者：杨程
// 创建时间：2021/12/10 16:52:34
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
    public class AddNewFunctionViewModel:BaseCRUDVM<FunctionClass>
    {
        private IDialogMessage Dialog { get; set; }
        public AddNewFunctionViewModel(IDataContext dc,IDialogMessage dialog):base(dc)
        {
            Dialog = dialog;
            //构造函数

            Title = "新增功能模块";
        }

        protected override void BaseCRUDInit()
        {
            Systems = new ObservableCollection<SystemClass>();

            Modules= new ObservableCollection<ModuleClass>();

            NewMethodClass = new MethodClass();

            GetData();
        }

        #region 属性
        public ObservableCollection<SystemClass> Systems { get; set; }

        public SystemClass SelectSystem { get; set; }

        public ObservableCollection<ModuleClass> Modules { get; set; }

        public ModuleClass SelectModule { get; set; }


        private MethodClass _NewMethodClass;

        public MethodClass NewMethodClass
        {
            get { return _NewMethodClass; }
            set { _NewMethodClass = value; DoNotify(); }
        }

        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        private void GetData()
        {
            Systems.Clear();

            var systems = DC.Set<SystemClass>().ToList();

            foreach (var item in systems)
            {
                Systems.Add(item);
            }

        }
        #endregion

        #region 命令
        public override RelayCommand SaveCommand => new RelayCommand(() => 
        {
            if (SelectSystem != null && SelectModule != null)
            {
                Entity.SystemClassId = SelectSystem.ID;
                Entity.SystemClassName = SelectSystem.SystemName;
                Entity.ModuleClassId = SelectModule.ID;
                Entity.ModuleClassName = SelectModule.ModuleName;

                if (!string.IsNullOrEmpty(Entity.FunctionName))
                {
                    using (var trans = DC.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in Entity.methods)
                            {
                                item.SystemClassId = Entity.SystemClassId;
                                item.SystemClassName= Entity.SystemClassName;

                                item.ModuleClassId = Entity.ModuleClassId;
                                item.ModuleClassName = Entity.ModuleClassName;

                                DC.AddEntity(item);
                            }

                            DC.AddEntity(Entity);

                            DC.SaveChanges();

                            trans.Commit();

                            ((Window)View).Close();
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


        public RelayCommand<SystemClass> SelectedSystemChanged => new RelayCommand<SystemClass>((s) => 
        {
            if (s != null)
            {
                Modules.Clear();

                var modules = DC.Set<ModuleClass>().Where(w=>w.SystemClassId==s.ID).ToList();
                foreach (var module in modules)
                {
                    Modules.Add(module);
                }
            }
        });

        public RelayCommand AddNewMethodCommand => new RelayCommand(() => 
        {
            NewMethodClass = new MethodClass();
        });

        public RelayCommand InsertNewMethodCommand => new RelayCommand(() => 
        {
            

            NewMethodClass.FunctionClassId = Entity.ID;
            NewMethodClass.FunctionClassName = Entity.FunctionName;

            if (!string.IsNullOrEmpty(NewMethodClass.MethodName))
            {
                if (!Entity.methods.Any(a => a.ID == NewMethodClass.ID))
                {
                    Entity.methods.Add(NewMethodClass);
                }

            }
        });
        #endregion
    }
}
