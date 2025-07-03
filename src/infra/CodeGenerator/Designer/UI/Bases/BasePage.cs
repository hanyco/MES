using System.Windows;
using System.Windows.Controls;

namespace CodeGenerator.Designer.UI.Bases
{
    public class BasePage : Control
    {
        static BasePage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasePage),
                new FrameworkPropertyMetadata(typeof(BasePage)));
        }

        // SidebarContent DependencyProperty
        public static readonly DependencyProperty SidebarContentProperty =
            DependencyProperty.Register("SidebarContent", typeof(object), typeof(BasePage), new PropertyMetadata(null));

        public object SidebarContent
        {
            get => GetValue(SidebarContentProperty);
            set => SetValue(SidebarContentProperty, value);
        }

        // ToolbarContent DependencyProperty
        public static readonly DependencyProperty ToolbarContentProperty =
            DependencyProperty.Register("ToolbarContent", typeof(object), typeof(BasePage), new PropertyMetadata(null));

        public object ToolbarContent
        {
            get => GetValue(ToolbarContentProperty);
            set => SetValue(ToolbarContentProperty, value);
        }

        // EntityDesignerContent DependencyProperty
        public static readonly DependencyProperty EntityDesignerContentProperty =
            DependencyProperty.Register("EntityDesignerContent", typeof(object), typeof(BasePage), new PropertyMetadata(null));

        public object EntityDesignerContent
        {
            get => GetValue(EntityDesignerContentProperty);
            set => SetValue(EntityDesignerContentProperty, value);
        }

        // SourceCodeContent DependencyProperty
        public static readonly DependencyProperty SourceCodeContentProperty =
            DependencyProperty.Register("SourceCodeContent", typeof(object), typeof(BasePage), new PropertyMetadata(null));

        public object SourceCodeContent
        {
            get => GetValue(SourceCodeContentProperty);
            set => SetValue(SourceCodeContentProperty, value);
        }
    }
}
