using System.Windows;
using System.Windows.Controls;

namespace EMV_Parser
{
    public static class TreeViewExtensions
    {
        public static readonly DependencyProperty ExpandAllNodesProperty = DependencyProperty.RegisterAttached(
            "ExpandAllNodes",
            typeof(bool),
            typeof(TreeViewExtensions),
            new UIPropertyMetadata(false, OnExpandAllNodesChanged));

        public static bool GetExpandAllNodes(DependencyObject obj)
        {
            return (bool)obj.GetValue(ExpandAllNodesProperty);
        }

        public static void SetExpandAllNodes(DependencyObject obj, bool value)
        {
            obj.SetValue(ExpandAllNodesProperty, value);
        }

        private static void OnExpandAllNodesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TreeView treeView && e.NewValue is bool expandAllNodes)
            {
                treeView.Loaded += (s, ev) =>
                {
                    foreach (var item in treeView.Items)
                    {
                        if (item is TreeViewItem treeViewItem)
                        {
                            treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, expandAllNodes);
                        }
                    }
                };
            }
        }
    }
}
