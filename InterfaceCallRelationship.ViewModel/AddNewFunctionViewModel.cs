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

        }

        private bool IsEdit { get; set; }=false;

        
        public override void PassData(object obj)
        {
            /*
             * 此处获取MainViewModel传递过来的值，如果没有传值，那么就是新增
             */

            FunctionClass current=obj as FunctionClass;
            if (current != null)
            {
                //如果是修改的话，此处需要到数据库中将systemclass数据根据这个id查出来
                var system=DC.Set<SystemClass>().First(f=>f.ID==current.SystemClassId);
                //传入这个命令中
                SelectedSystemChangedCommand(system);
                //通过框架自带的设置当前页面的实体
                SetEntity(current);

                Title = "修改功能模块";

                IsEdit =true;
            }
            else
            {
                Title = "新增功能模块";
            }
        }


        bool IsOk = false;

        public override object GetResult()
        {
            /*
             * 根据情况返回数据，此处图简单，就传了个bool回去
             * 关闭窗体的时候生效
             */
            return IsOk;
        }

        
        protected override void BaseCRUDInit()
        {
            Systems = new ObservableCollection<SystemClass>();

            Modules= new ObservableCollection<ModuleClass>();

            NewMethodClass = new MethodClass();

            GetData();
        }

        #region 属性
        /// <summary>
        /// 系统下拉框的列表
        /// </summary>
        public ObservableCollection<SystemClass> Systems { get; set; }
        /// <summary>
        /// 系统下拉框中选择的系统模块
        /// </summary>
        public SystemClass SelectSystem { get; set; }

        /// <summary>
        /// 模块下拉框的列表
        /// </summary>
        public ObservableCollection<ModuleClass> Modules { get; set; }
        /// <summary>
        /// 模块下拉框中选择的模块
        /// </summary>
        public ModuleClass SelectModule { get; set; }


        private MethodClass _NewMethodClass;
        /// <summary>
        /// 新增方法，因为随时会进行变更，所以改成完整属性并添加属性通知
        /// </summary>
        public MethodClass NewMethodClass
        {
            get { return _NewMethodClass; }
            set { _NewMethodClass = value; DoNotify(); }
        }

        #endregion

        #region 公共方法

        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化的时候，获取系统下拉框的数据
        /// </summary>
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
        /// <summary>
        /// 保存命令
        /// </summary>
        public override RelayCommand SaveCommand => new RelayCommand(() => 
        {
            //判断是否选择了系统和模块
            if (SelectSystem != null && SelectModule != null)
            {
                //赋值
                Entity.SystemClassId = SelectSystem.ID;
                Entity.SystemClassName = SelectSystem.SystemName;
                Entity.ModuleClassId = SelectModule.ID;
                Entity.ModuleClassName = SelectModule.ModuleName;

                //判断是否填写功能名称
                if (!string.IsNullOrEmpty(Entity.FunctionName))
                {
                    //开启事务
                    using (var trans = DC.Database.BeginTransaction())
                    {
                        try
                        {
                            //读取数据库获取到这个功能ID下的全部方法，然后在Entity的Methods中排除掉已存在数据库中的数据，剩下的就是本次新增的
                            DC.Set<MethodClass>().Where(w => w.FunctionClassId == Entity.ID).ToList().ForEach(f => Entity.methods.Remove(f));

                            //翔数据库中插入新增的数据
                            foreach (var item in Entity.methods)
                            {
                                item.SystemClassId = Entity.SystemClassId;
                                item.SystemClassName= Entity.SystemClassName;

                                item.ModuleClassId = Entity.ModuleClassId;
                                item.ModuleClassName = Entity.ModuleClassName;

                                DC.AddEntity(item);
                            }


                            if (IsEdit)
                            {
                                //DC.UpdateEntity(Entity);
                            }
                            else
                            {
                                DC.AddEntity(Entity);
                            }

                            

                            DC.SaveChanges();
                            //事务提交
                            trans.Commit();
                            IsOk=true;

                            ((Window)View).Close();//这里的View在DialogWindow窗体创建的时候，就通过框架绑定了，所以此处可直接这样使用进行窗体的关闭
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


        /// <summary>
        /// 选择系统之后的数据变更命令
        /// </summary>
        public RelayCommand<SystemClass> SelectedSystemChanged => new RelayCommand<SystemClass>(SelectedSystemChangedCommand);

        /// <summary>
        /// 选择系统之后的数据变更命令
        /// </summary>
        /// <param name="s"></param>
        private void SelectedSystemChangedCommand(SystemClass s)
        {
            //判断一下传入的SystemClass是否为空
            if (s != null)
            {
                //清空模块列表
                Modules.Clear();

                //根据传入的SystemClass的id去数据库查关联的模块
                var modules = DC.Set<ModuleClass>().Where(w => w.SystemClassId == s.ID).ToList();
                //添加进模块列表中
                foreach (var module in modules)
                {
                    Modules.Add(module);
                }
            }
        }

        /// <summary>
        /// 点击新增的时候，会new一个methodclass类覆盖掉之前的
        /// </summary>
        public RelayCommand AddNewMethodCommand => new RelayCommand(() => 
        {
            NewMethodClass = new MethodClass();
        });

        /// <summary>
        /// 插入新的方法命令
        /// </summary>
        public RelayCommand InsertNewMethodCommand => new RelayCommand(() => 
        {
            //针对新增的方法，依次赋值

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
