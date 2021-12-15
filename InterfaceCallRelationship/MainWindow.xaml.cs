using InterfaceCallRelationship.Model;
using InterfaceCallRelationship.Styles.CustomControl.ShowViewBoxControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vampirewal.Core.SimpleMVVM;
using Vampirewal.Core.WpfTheme.WindowStyle;

namespace InterfaceCallRelationship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<MethodClass>(this, "CreateTopThumb", CreateTopThumb);
        }

        private void CreateTopThumb(MethodClass methodclass)
        {
            //canvas.Children.Clear();

            //ClassNode node= new ClassNode();
            //node.Reference = true;
            //node.TitleName = methodclass.FunctionClassName;
            //node.ShowMethodName = methodclass.MethodName;
            //node.IsTopNode = true;

            //Canvas.SetTop(node, 2);
            //Canvas.SetLeft(node, 2);
            //Canvas.SetZIndex(node, 999);

            


            //canvas.Children.Add(node);
        }

        private void ClassNode_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb myThumb = (Thumb)sender;

            double nTop = Canvas.GetTop(myThumb) + e.VerticalChange;
            double nLeft = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            Canvas.SetTop(myThumb, nTop);
            Canvas.SetLeft(myThumb, nLeft);
        }
    }
}
