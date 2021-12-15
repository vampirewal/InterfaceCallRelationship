using InterfaceCallRelationship.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

namespace InterfaceCallRelationship.Styles.CustomControl.ShowViewBoxControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:InterfaceCallRelationship.Styles.CustomControl.ShowViewBoxControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:InterfaceCallRelationship.Styles.CustomControl.ShowViewBoxControl;assembly=InterfaceCallRelationship.Styles.CustomControl.ShowViewBoxControl"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ShowViewBox/>
    ///
    /// </summary>
    public class ShowCallRelationshipPanel : Canvas
    {
        static ShowCallRelationshipPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShowCallRelationshipPanel), new FrameworkPropertyMetadata(typeof(ShowCallRelationshipPanel)));
        }

        //public ObservableCollection<ClassNode> ClassNodes { get; set; }

        public ShowCallRelationshipPanel()
        {

        }



        [Bindable(true)]
        public ObservableCollection<MethodNode> ClassNodes
        {
            get { return (ObservableCollection<MethodNode>)GetValue(ClassNodesProperty); }
            set { SetValue(ClassNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClassNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassNodesProperty =
            DependencyProperty.Register("ClassNodes", typeof(ObservableCollection<MethodNode>), typeof(ShowCallRelationshipPanel), new PropertyMetadata(default,OnClassNodesChangedCallBack));

        

        private static void OnClassNodesChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowCallRelationshipPanel panel=d as ShowCallRelationshipPanel;

            panel.test();

            
        }

        public void test()
        {
            if (ClassNodes is INotifyCollectionChanged)
            {
                (ClassNodes as INotifyCollectionChanged).CollectionChanged -= ShowCallRelationshipPanel_CollectionChanged;
                (ClassNodes as INotifyCollectionChanged).CollectionChanged += ShowCallRelationshipPanel_CollectionChanged;
            }
        }

        private List<ClassNode> ClassNodeDic=new List<ClassNode>();

        private void ShowCallRelationshipPanel_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        foreach (var item in e.NewItems)
                        {
                            var methodnode = item as MethodNode;

                            if (methodnode.IsTopNode)
                            {
                                this.Children.Clear();
                                ClassNodeDic.Clear();
                            }

                            if (!methodnode.IsTopNode)
                            {
                                foreach (var target in methodnode.TargetList)
                                {
                                    //ClassNode node = new ClassNode();
                                    ////node.Reference = true;
                                    //node.TitleName = methodnode.FunctionName;
                                    //node.ShowMethodName = methodnode.MethodName;
                                    //node.IsTopNode = methodnode.IsTopNode;
                                    //node.Reference = methodnode.IsSource;
                                    //node.Referenced = methodnode.IsReferenced;
                                }

                                
                            }


                            ClassNode node = new ClassNode();
                            //node.Reference = true;
                            node.TitleName = methodnode.FunctionName;
                            node.ShowMethodName = methodnode.MethodName;
                            node.IsTopNode = methodnode.IsTopNode;
                            node.Reference = methodnode.IsSource;
                            node.Referenced = methodnode. IsReferenced;
                            node.Id = methodnode.Id;
                            node.ParentId = methodnode.ParentId;

                            if (methodnode.IsSource)
                            {
                                node.ShowUpCommand = ShowUpCommand;
                                node.ShowCommandParameter = methodnode;
                            }

                            if (methodnode.IsReferenced)
                            {
                                node.ShowDownCommand = ShowDownCommand;
                                node.ShowCommandParameter = methodnode;

                                
                            }


                            if (!methodnode.IsTopNode)
                            {
                                /*
                                 * 如果不是顶级节点，那么说明就是从顶级节点展开的
                                 * 那么在此处进行拖拽事件的添加以及连线的渲染
                                 */

                                foreach (var target in methodnode.TargetList)
                                {

                                }

                                node.DragStarted += ClassNode_DragStarted;
                                node.DragDelta += ClassNode_DragDelta;
                                node.DragCompleted += ClassNode_DragCompleted;

                                Canvas.SetTop(node, 200);
                                Canvas.SetLeft(node, 200);
                                Canvas.SetZIndex(node, 1);
                            }
                            else
                            {
                                /*
                                 * 此处是顶级节点的canvas设置
                                 */

                                Canvas.SetTop(node, this.ActualHeight/2);
                                Canvas.SetLeft(node, this.ActualWidth/2);
                                Canvas.SetZIndex(node, 999);
                            }




                            ClassNodeDic.Add( node);

                            this.Children.Add(node);
                        }

                        if (ClassNodeDic.Count>0)
                        {
                            foreach (var item in ClassNodeDic)
                            {
                                if (item.IsTopNode)
                                {
                                    continue;
                                }

                                var node=ClassNodeDic.Find(f=>f.Id==item.ParentId);

                                UpdateThumb(node, item);
                            }
                        }


                        break;
                        case NotifyCollectionChangedAction.Remove:
                        break;
                        default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            var node = ClassNodeDic.Find(f => f.IsTopNode);
            if (node != null)
            {
                Canvas.SetTop(node, this.ActualHeight / 2);
                Canvas.SetLeft(node, this.ActualWidth / 2);
            }
            
        }

        private void UpdateThumb(ClassNode node1, ClassNode node2, bool IsNew=true)
        {
            if (node2.OwnPath!=null&& IsNew)
            {
                return;
            }

            var point1 = new Point((double)node1.GetValue(Canvas.LeftProperty) + node1.Width, (double)node1.GetValue(Canvas.TopProperty) + node1.Height / 2);
            var point2 =  new Point((double)node2.GetValue(Canvas.LeftProperty), (double)node2.GetValue(Canvas.TopProperty) + node2.Height / 2);

            if (IsNew)
            {
                System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();

                string sData = "M 0,0 c 200,0 100,300 300,300";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));

                path.Data = (Geometry)converter.ConvertFrom(sData);
                path.Height = 200;
                path.Stretch = Stretch.Fill;
                path.Stroke = Brushes.Green;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeThickness = 2;
                path.Width = 200;
                path.RenderTransformOrigin = new Point(0.5, 0.5);
                //path.RenderTransform

                ScaleTransform scale = new ScaleTransform();
                path.RenderTransform = scale;

                path.SetValue(Canvas.LeftProperty, Math.Min(point1.X, point2.X));
                path.SetValue(Canvas.TopProperty, Math.Min(point1.Y, point2.Y));
                path.Width = Math.Abs(point1.X - point2.X);
                path.Height = Math.Abs(point1.Y - point2.Y);
                if (point1.X < point2.X)
                {
                    scale.ScaleX = point1.Y < point2.Y ? 1 : -1;
                }
                else
                {
                    scale.ScaleX = point1.Y < point2.Y ? -1 : 1;

                }

                this.Children.Add(path);

                node2.OwnPath = path;
            }
            else
            {
                node2.OwnPath.SetValue(Canvas.LeftProperty, Math.Min(point1.X, point2.X));
                node2.OwnPath.SetValue(Canvas.TopProperty, Math.Min(point1.Y, point2.Y));
                node2.OwnPath.Width = Math.Abs(point1.X - point2.X);
                node2.OwnPath.Height = Math.Abs(point1.Y - point2.Y);

                ScaleTransform scale = new ScaleTransform();
                node2.OwnPath.RenderTransform = scale;

                if (point1.X < point2.X)
                {
                    scale.ScaleX = point1.Y < point2.Y ? 1 : -1;
                }
                else
                {
                    scale.ScaleX = point1.Y < point2.Y ? -1 : 1;

                }
            }

            
        }

        private void UpdateTopNode(ClassNode node)
        {

        }

        private void ClassNode_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //Thumb curThumb = (Thumb)sender;
        }

        private void ClassNode_DragStarted(object sender, DragStartedEventArgs e)
        {
            //Thumb curThumb = (Thumb)sender;
        }

        private void ClassNode_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ClassNode curThumb = (ClassNode)sender;

            var ParentNode=ClassNodeDic.FirstOrDefault(x => x.Id == curThumb.ParentId);
            if (ParentNode != null)
            {


                double nTop = Canvas.GetTop(curThumb) + e.VerticalChange;
                double nLeft = Canvas.GetLeft(curThumb) + e.HorizontalChange;
                Canvas.SetTop(curThumb, nTop);
                Canvas.SetLeft(curThumb, nLeft);

                UpdateThumb(ParentNode, curThumb, false);
            }

           
        }


        public ICommand ShowUpCommand
        {
            get { return (ICommand)GetValue(ShowUpCommandProperty); }
            set { SetValue(ShowUpCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowUpCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowUpCommandProperty =
            DependencyProperty.Register("ShowUpCommand", typeof(ICommand), typeof(ShowCallRelationshipPanel), new PropertyMetadata(null));


        public ICommand ShowDownCommand
        {
            get { return (ICommand)GetValue(ShowDownCommandProperty); }
            set { SetValue(ShowDownCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDownCommandProperty =
            DependencyProperty.Register("ShowDownCommand", typeof(ICommand), typeof(ShowCallRelationshipPanel), new PropertyMetadata(null));


    }


    public class ClassNode : Thumb
    {
        public string ParentId { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// 是否有引用
        /// </summary>
        public bool Reference { get; set; }

        /// <summary>
        /// 是否有被引用
        /// </summary>
        public bool Referenced { get; set; }

        ///// <summary>
        ///// 是否顶层node
        ///// </summary>
        //public bool IsTopNode { get; set; }

        public System.Windows.Shapes.Path OwnPath { get; set; }


        public Button ShowUp { get; set; }

        public Button ShowDown { get; set; }


        static ClassNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassNode), new FrameworkPropertyMetadata(typeof(ClassNode)));
        }

        private ResourceDictionary res
        {
            get
            {
                return new ResourceDictionary() { Source = new Uri("pack://application:,,,/InterfaceCallRelationship;component/Styles/CustomControl/ShowViewBoxControl/ShowViewBoxStyle.xaml", UriKind.RelativeOrAbsolute) };
            }
        }

        public ClassNode()
        {
            var BaseStyle = res["classNode"] as Style;

            this.Style = BaseStyle;

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

             ShowUp = this.Template.FindName("ShowUp", this) as Button;
            ShowUp.Command = ShowUpCommand;
            ShowUp.CommandParameter = ShowCommandParameter;

            if (this.Reference)
            {
                ShowUp.IsEnabled = true;
                
            }
            else
            {
                ShowUp.IsEnabled = false;
                
            }


             ShowDown = this.Template.FindName("ShowDown", this) as Button;
            ShowDown.Command = ShowDownCommand;
            ShowDown.CommandParameter = ShowCommandParameter;
            if (this.Referenced)
            {
                ShowDown.IsEnabled = true;
            }
            else
            {
                ShowDown.IsEnabled = false;
            }
                        
        }

        public override void EndInit()
        {
            base.EndInit();
            ShowUpCommand?.Execute(ShowCommandParameter);
            ShowDownCommand?.Execute(ShowCommandParameter);
        }

        #region 依赖属性

        #region 是否最顶层

        /// <summary>
        /// 是否最顶层node
        /// </summary>
        public bool IsTopNode
        {
            get { return (bool)GetValue(IsTopNodeProperty); }
            set { SetValue(IsTopNodeProperty, value); }
        }
        /// <summary>
        /// 是否最顶层node
        /// </summary>
        public static readonly DependencyProperty IsTopNodeProperty =
            DependencyProperty.Register("IsTopNode", typeof(bool), typeof(ClassNode), new PropertyMetadata(true));

        #endregion


        public string TitleName
        {
            get { return (string)GetValue(TitleNameProperty); }
            set { SetValue(TitleNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleNameProperty =
            DependencyProperty.Register("TitleName", typeof(string), typeof(ClassNode), new PropertyMetadata(""));



        public string ShowMethodName
        {
            get { return (string)GetValue(ShowMethodNameProperty); }
            set { SetValue(ShowMethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMethodNameProperty =
            DependencyProperty.Register("ShowMethodName", typeof(string), typeof(ClassNode), new PropertyMetadata(""));



        public ICommand ShowUpCommand
        {
            get { return (ICommand)GetValue(ShowUpCommandProperty); }
            set { SetValue(ShowUpCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowUpCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowUpCommandProperty =
            DependencyProperty.Register("ShowUpCommand", typeof(ICommand), typeof(ClassNode), new PropertyMetadata(null));


        public ICommand ShowDownCommand
        {
            get { return (ICommand)GetValue(ShowDownCommandProperty); }
            set { SetValue(ShowDownCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDownCommandProperty =
            DependencyProperty.Register("ShowDownCommand", typeof(ICommand), typeof(ClassNode), new PropertyMetadata(null));


        public object ShowCommandParameter
        {
            get { return (object)GetValue(ShowCommandParameterProperty); }
            set { SetValue(ShowCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowUpCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCommandParameterProperty =
            DependencyProperty.Register("ShowUpCommandParameter", typeof(object), typeof(ClassNode), new PropertyMetadata(0,ShowCommandParameterPropertyChangedCallBack));

        private static void ShowCommandParameterPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cur = d as ClassNode;
            cur.ShowCommandParameter = e.NewValue;
        }



        #endregion
    }



}
