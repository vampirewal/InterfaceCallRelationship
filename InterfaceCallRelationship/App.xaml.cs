using InterfaceCallRelationship.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Vampirewal.Core.SimpleMVVM;

namespace InterfaceCallRelationship
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : VampirewalApplication
    {
        /*
         * 整个程序都使用了我自己封装的类，可以在Nuget中搜索Vampirewal.Core安装
         * https://blog.csdn.net/weixin_42806176/article/details/120705323 这个是介绍博客，没时间写完介绍了。。。
         * 如果在二次开发的时候遇到问题，可以添加我的QQ：235160615，或者发邮件：235160615@qq.com
         * 接近年底了，公司的事情很多。。。
         */

        protected override void OnStartup(StartupEventArgs e)
        {
            //这里使用了我自己封装的Application，如果用的是VS2022可以直接打开看源码，基本是就是提前注册了一些打开窗体的消息

            SetAssembly(Assembly.GetExecutingAssembly());//传入当前View这个项目的程序集，方便后面使用打开窗体的消息
            base.OnStartup(e);
            OpenWinodw(ViewKeys.MainView);
        }
    }
}
