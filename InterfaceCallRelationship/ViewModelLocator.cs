#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：ViewModelLocator
// 创 建 者：杨程
// 创建时间：2021/12/10 9:31:20
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using InterfaceCallRelationship.DataAccess;
using InterfaceCallRelationship.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vampirewal.Core.Components;
using Vampirewal.Core.Interface;
using Vampirewal.Core.SimpleMVVM;

namespace InterfaceCallRelationship
{
    public class ViewModelLocator : ViewModelLocatorBase
    {
        public override void InitRegisterService()
        {
            CustomIoC.Instance.Register<IDataContext, ICR_DataContext>();
            base.InitRegisterService();
        }

        public override void InitLocator()
        {
            base.InitLocator();
        }

        public override void InitRegisterViewModel()
        {
            CustomIoC.Instance.Register<MainViewModel>();
            CustomIoC.Instance.Register<SettingViewModel>();
            CustomIoC.Instance.Register<AddNewFunctionViewModel>();
        }


        public MainViewModel MainViewModel => CustomIoC.Instance.GetInstance<MainViewModel>();

        public SettingViewModel SettingViewModel => CustomIoC.Instance.GetInstance<SettingViewModel>();

        public AddNewFunctionViewModel AddNewFunctionViewModel=>CustomIoC.Instance.GetInstanceWithoutCaching<AddNewFunctionViewModel>();
    }
}
