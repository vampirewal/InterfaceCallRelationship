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
    /// 

    
    public class ShowCallRelationshipPanel : Canvas
    {
        /*
         * 这个是本文的重点，整个程序的价值全在这里了
         */

        static ShowCallRelationshipPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShowCallRelationshipPanel), new FrameworkPropertyMetadata(typeof(ShowCallRelationshipPanel)));
        }

        //public ObservableCollection<ClassNode> ClassNodes { get; set; }

        public ShowCallRelationshipPanel()
        {

        }


        #region 给Canvas添加数据源，可在界面上使用这个自定义控件，然后通过绑定的方式，和MainViewModel中的Methods进行关联

        [Bindable(true)]
        public ObservableCollection<MethodNode> ClassNodes
        {
            get { return (ObservableCollection<MethodNode>)GetValue(ClassNodesProperty); }
            set { SetValue(ClassNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClassNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClassNodesProperty =
            DependencyProperty.Register("ClassNodes", typeof(ObservableCollection<MethodNode>), typeof(ShowCallRelationshipPanel), new PropertyMetadata(default, OnClassNodesChangedCallBack));

        //上面是实现依赖属性
        
        //响应数据源的变化，只有在界面初始化的时候会进来一次，所以进来的时候，就要给这个控件添加数据源变化的事件监听
        private static void OnClassNodesChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowCallRelationshipPanel panel = d as ShowCallRelationshipPanel;

            panel.ShowCallRelationshipPanelCollectionChanged();


        }
        //监听就在这里
        public void ShowCallRelationshipPanelCollectionChanged()
        {
            if (ClassNodes is INotifyCollectionChanged)
            {
                (ClassNodes as INotifyCollectionChanged).CollectionChanged -= ShowCallRelationshipPanel_CollectionChanged;
                (ClassNodes as INotifyCollectionChanged).CollectionChanged += ShowCallRelationshipPanel_CollectionChanged;
            }
        }

        /// <summary>
        /// 这个list是用来存界面上显示的node的，因为后续涉及到拖拽变化线条等等，需要从这里取数
        /// </summary>
        private List<ClassNode> ClassNodeDic = new List<ClassNode>();


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

                            //如果新增的这个节点是顶层节点的话，就先清空界面上的节点
                            if (methodnode.IsTopNode)
                            {
                                this.Children.Clear();
                                ClassNodeDic.Clear();
                            }
                                                      

                            //然后在这里新增
                            ClassNode node = new ClassNode();
                            node.TitleName = methodnode.FunctionName;
                            node.ShowMethodName = methodnode.MethodName;
                            node.IsTopNode = methodnode.IsTopNode;
                            node.Reference = methodnode.IsSource;
                            node.Referenced = methodnode.IsReferenced;
                            node.Id = methodnode.Id;
                            node.ParentId = methodnode.ParentId;

                            if (methodnode.IsSource)
                            {
                                //添加按钮的ICommand命令
                                node.ShowUpCommand = ShowUpCommand;
                                node.ShowCommandParameter = methodnode;
                            }

                            if (methodnode.IsReferenced)
                            {
                                //添加按钮的ICommand命令
                                node.ShowDownCommand = ShowDownCommand;
                                node.ShowCommandParameter = methodnode;

                            }

                            //添加节点的拖拽事件
                            node.DragStarted += ClassNode_DragStarted;
                            node.DragDelta += ClassNode_DragDelta;
                            node.DragCompleted += ClassNode_DragCompleted;
                            node.MouseRightButtonDown += Node_MouseRightButtonDown;
                            node.MouseRightButtonUp += Node_MouseRightButtonUp;

                            if (!methodnode.IsTopNode)
                            {
                                /*
                                 * 如果不是顶级节点，那么说明就是从顶级节点展开的
                                 */

                                //获取到这个节点的父级节点
                                var ParentNode=ClassNodeDic.Find(f=>f.Id==methodnode.ParentId);

                                //if (methodnode.IsSource)
                                //{
                                //    Canvas.SetLeft(node,( Canvas.GetLeft(ParentNode)-200)<0?0: Canvas.GetLeft(ParentNode) - 200);//往左边放200
                                //}

                                //if (methodnode.IsReferenced)
                                //{
                                //    Canvas.SetLeft(node, Canvas.GetLeft(ParentNode) + 200);//往左边放200
                                //}
                                Canvas.SetLeft(node, Canvas.GetLeft(ParentNode) + 250);//往左边放200
                                Canvas.SetTop(node, Canvas.GetTop(ParentNode)+100);//设置为同一行
                                Canvas.SetZIndex(node, 1);

                            }
                            else
                            {
                                /*
                                 * 此处是顶级节点的canvas设置，初始化在Canvas的中间
                                 */

                                Canvas.SetTop(node, this.ActualHeight / 2);
                                Canvas.SetLeft(node, this.ActualWidth / 2);
                                Canvas.SetZIndex(node, 999);
                            }


                            //将这个节点添加进这个列表中
                            ClassNodeDic.Add(node);

                            //将节点添加进界面中
                            this.Children.Add(node);
                        }

                        //绘制连线
                        if (ClassNodeDic.Count > 0)
                        {
                            foreach (var item in ClassNodeDic)
                            {
                                if (item.IsTopNode)
                                {
                                    continue;
                                }

                                var node = ClassNodeDic.Find(f => f.Id == item.ParentId);

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

        


        /// <summary>
        /// 绘制连线
        /// </summary>
        /// <param name="node1">父节点</param>
        /// <param name="node2">自己而几点</param>
        /// <param name="IsNew">是否新增</param>
        private void UpdateThumb(ClassNode node1, ClassNode node2, bool IsNew = true)
        {
            if (node2.OwnPath != null && IsNew)
            {
                return;
            }

            var point1 = new Point((double)node1.GetValue(Canvas.LeftProperty) + node1.Width, (double)node1.GetValue(Canvas.TopProperty) + node1.Height / 2);
            var point2 = new Point((double)node2.GetValue(Canvas.LeftProperty), (double)node2.GetValue(Canvas.TopProperty) + node2.Height / 2);

            if (IsNew)
            {
                System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();

                string sData = "M 0,0 c 200,0 100,300 300,300";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));

                path.Data = (Geometry)converter.ConvertFrom(sData);
                path.Height = 200;
                path.Stretch = Stretch.Fill;
                path.Stroke = Brushes.Green;//可以修改颜色，后面可以将这个做成依赖属性，放到xaml中进行绑定
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeThickness = 2;//线条粗细
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

                

                //将线条添加进canvas中显示
                this.Children.Add(path);
                //将线条传入自身节点中储存
                node2.OwnPath = path;
            }
            else if(node2.OwnPath != null)
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

        /// <summary>
        /// 顶层节点的拖拽线条移动
        /// </summary>
        /// <param name="node"></param>
        private void UpdateTopNode(ClassNode node)
        {
            var childs = ClassNodeDic.Where(w => w.ParentId == node.Id).ToList();
            //因为从顶层节点展开的可能会有很多条，所以此处使用foreach进行每条节点的更新
            foreach (var child in childs)
            {
                UpdateThumb(node, child, false);
            }
        }

        /// <summary>
        /// 节点拖拽完事件，可自行修改，比如修改颜色啊，触发什么之类的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClassNode_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ClassNode curThumb = (ClassNode)sender;
            if (!curThumb.IsTopNode)
            {
                Canvas.SetZIndex(curThumb, 1);
            }
            
        }

        /// <summary>
        /// 节点开始拖拽事件，可自行修改，比如修改颜色啊，触发什么之类的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClassNode_DragStarted(object sender, DragStartedEventArgs e)
        {
            ClassNode curThumb = (ClassNode)sender;
            if (!curThumb.IsTopNode)
            {
                Canvas.SetZIndex(curThumb, 10);
            }
        }

        /// <summary>
        /// 节点拖拽中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClassNode_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ClassNode curThumb = (ClassNode)sender;

            double nTop = Canvas.GetTop(curThumb) + e.VerticalChange;
            double nLeft = Canvas.GetLeft(curThumb) + e.HorizontalChange;
            Canvas.SetTop(curThumb, nTop);
            Canvas.SetLeft(curThumb, nLeft);

            //获取父节点，如果不为空，那么在拖拽中更新线条
            var ParentNode = ClassNodeDic.FirstOrDefault(x => x.Id == curThumb.ParentId);
            if (ParentNode != null)
            {
                UpdateThumb(ParentNode, curThumb, false);
            }

            //获取子节点，存在N条的情况下，每条都要进行线条位置的更新
            var ChildNode = ClassNodeDic.Where(w => w.ParentId == curThumb.Id).ToList();
            foreach (var item in ChildNode)
            {
                UpdateThumb(curThumb, item, false);
            }

            //如果是顶层节点，也是一样的逻辑，自己拖拽了，也要更新自己子节点的线条
            if (curThumb.IsTopNode)
            {
                UpdateTopNode(curThumb);
            }
        }

        #region ClassNode点击右键查看引用关系连线变色
        private void Node_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ClassNode cur)
            {
                //cur.OwnPath.Stroke = Brushes.Red;
                if (!string.IsNullOrEmpty(cur.ParentId))
                {
                    ChangeClassNodePathColor(cur, ClassNodeDic, false);
                }
            }
        }

        private void Node_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ClassNode cur)
            {
                //cur.OwnPath.Stroke = Brushes.Red;
                if (!string.IsNullOrEmpty(cur.ParentId))
                {
                    ChangeClassNodePathColor(cur, ClassNodeDic, true);
                }
            }
        }

        private void ChangeClassNodePathColor(ClassNode node, List<ClassNode> nodes, bool IsDownOrUp)
        {
            if (IsDownOrUp)
            {
                if (!string.IsNullOrEmpty(node.ParentId))
                {
                    node.OwnPath.Stroke = Brushes.Red;

                    var parent = nodes.FirstOrDefault(f => f.Id == node.ParentId);
                    if (parent != null)
                    {
                        ChangeClassNodePathColor(parent, nodes, true);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(node.ParentId))
                {
                    node.OwnPath.Stroke = Brushes.Green;

                    var parent = nodes.FirstOrDefault(f => f.Id == node.ParentId);
                    if (parent != null)
                    {
                        ChangeClassNodePathColor(parent, nodes, false);
                    }
                }
            }


        } 
        #endregion

        #endregion

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


        //canvas的尺寸变更，暂时没用，当初想用
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

        }
    }

    /// <summary>
    /// 节点
    /// </summary>
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

        /// <summary>
        /// 存储线条（顶层节点不存这个）
        /// </summary>
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
                ShowUp.Visibility = Visibility.Collapsed;
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
                ShowDown.Visibility = Visibility.Collapsed;
            }
                        
        }

        public override void EndInit()
        {
            base.EndInit();
            ShowUpCommand?.Execute(ShowCommandParameter);
            ShowDownCommand?.Execute(ShowCommandParameter);
        }

        #region 依赖属性（这些就不注释了，方便样式里面绑定的东西，属于基础了）

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
