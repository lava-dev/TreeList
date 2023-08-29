using Gamanet.C4.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfApp1.Base;
using WpfApp1.Base.Enums;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView
    {
        private MainWindowViewModel _viewModel;
        private _AppContext _appContext => (_AppContext)DataContext;
        private Action DataInitialized; // this is to codebehind
        private double _remainingWidth;

        public MainWindowView()
        {
            InitializeComponent();

            this.DataContextChanged += MainWindowView_DataContextChanged;
            this.DataInitialized = UpdateControl;
        }

        private void MainWindowView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = new MainWindowViewModel(_appContext, DataInitialized);
            RootContainer.DataContext = _viewModel;
        }

        private void GenerateData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.GetDataAndBuildTree();
        }

        private void ExpandAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ExpandCollapseButton.Content = "Collapse All";
            _viewModel.ExpandAll();
        }

        private void CollapseAll_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ExpandCollapseButton.Content = "Expand All";
            _viewModel.CollapseAll();
        }

        private void ExpandNext_Checked(object sender, RoutedEventArgs e)
        {
            var level = 1;
            ExpandCollapseNextButton.Content = $"Collapse Next (level = {level})";
            _viewModel.ExpandNext(level);
        }

        private void CollapseNext_Unchecked(object sender, RoutedEventArgs e)
        {
            var level = 1;
            ExpandCollapseNextButton.Content = $"Expand Next (level = {level})";
            _viewModel.CollapseNext(level);
        }

        private void CalculateForCell_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CalculateForCell(1, 0);
        }

        public void ExpandNext_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender != null
                && sender is FrameworkElement fwkElem
                && fwkElem.DataContext is RowEntity rowEntity)
            {
                var row = rowEntity.Index;
                _viewModel.ExpandNext(row);
            }
        }

        public void CollapseNext_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender != null
                && sender is FrameworkElement fwkElem
                && fwkElem.DataContext is RowEntity rowEntity)
            {
                var row = rowEntity.Index;
                _viewModel.CollapseNext(row);
            }
        }

        private void Cell_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender != null
                && sender is Image image
                && image.DataContext is RowEntity rowEntity)
            {
                // checking modify permission
                if (rowEntity.IsModifyPermission) return;

                var row = rowEntity.Index;
                var column = (int)image.Tag;
                _viewModel.CalculateForCell(row, column);
            }
        }

        private void RightButtonCell_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender != null
                && sender is Image image
                && image.DataContext is RowEntity rowEntity)
            {
                var row = rowEntity.Index;
                var column = (int)image.Tag;
                var currentState = rowEntity[column];

                // checking modify permission
                if (rowEntity.IsModifyPermission)
                {
                    BuildContextMenu(image, currentState, column);
                }
                else
                {
                    BuildContextManuYouDontHaveRights(image);
                }
            }
        }

        private void BuildContextManuYouDontHaveRights(Image image)
        {
            var contextMenu = new ContextMenu();
            var contextMenuItem = new MenuItem();
            contextMenuItem.Style = (Style)Application.Current.TryFindResource("PermissionsStateChangeContextMenuItemStyle");
            contextMenuItem.Icon = CommandImageMaper.GetPermissionState(enPermissionStateCommand.DeniedDirect);
            contextMenuItem.Header = Translations.Instance.Translate($"txtYouDontHaveRightsForModifyngThisPermission");

            contextMenu.Items.Add(contextMenuItem);

            image.ContextMenu = contextMenu;
            contextMenu.Visibility = Visibility.Visible;
        }

        private void BuildContextMenu(Image image, byte currentState, int column)
        {
            var contextMenu = new ContextMenu();

            enPermissionStateCommand itemForDisable = enPermissionStateCommand.Inherited;
            if (currentState != 0 && currentState != 1)
            {
                // disable
                itemForDisable = (enPermissionStateCommand)currentState;
            }

            foreach (var permissionCommand in (enPermissionStateCommand[])Enum.GetValues(typeof(enPermissionStateCommand)))
            {
                var contextMenuItem = new MenuItem();
                contextMenuItem.Style = (Style)Application.Current.TryFindResource("PermissionsStateChangeContextMenuItemStyle");
                contextMenuItem.Icon = CommandImageMaper.GetPermissionState(permissionCommand);
                contextMenuItem.IsEnabled = itemForDisable != permissionCommand;
                contextMenuItem.Header = Translations.Instance.Translate($"txt{permissionCommand}");
                contextMenuItem.Click += ContextMenuItem_Click;
                contextMenuItem.Tag = new PermissionCommandInfo(column, permissionCommand);

                contextMenu.Items.Add(contextMenuItem);
            }

            image.ContextMenu = contextMenu;
            contextMenu.Visibility = Visibility.Visible;
        }

        private void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null
                && sender is MenuItem menuItem
                && menuItem.DataContext is RowEntity row
                && menuItem.Tag is PermissionCommandInfo infoItem)
            {
                _viewModel.ChangeCellToTargetState(row.Index, infoItem.ColumnIndex, infoItem.SelectedState);
            }
        }

        private void UpdateControl()
        {
            GridView gridView = new GridView();
            gridView.AllowsColumnReorder = false;

            GridViewColumn column1 = new GridViewColumn();
            column1.HeaderContainerStyle = (Style)TreeListControl.TryFindResource("HeaderContainerStyle");
            column1.HeaderTemplate = (DataTemplate)TreeListControl.TryFindResource("NameHeaderTemplate");
            var cellTemplate = (DataTemplate)TreeListControl.TryFindResource("NodeTemplate");

            column1.CellTemplate = cellTemplate;

            gridView.Columns.Add(column1);

            _remainingWidth = 0;
            foreach (var type in _appContext.ColumnDefinitionRepo.ColumnDefinitions)
            {
                // image template
                var imageFactory = new FrameworkElementFactory(typeof(Image));
                imageFactory.SetBinding(Image.SourceProperty,
                                        new Binding($"Cells[{_appContext.ColumnDefinitionRepo.TypeToColumnMapper[type.Key]}]"));
                imageFactory.SetValue(Image.WidthProperty, 20.0);
                imageFactory.SetValue(Image.HeightProperty, 20.0);
                imageFactory.SetValue(Image.TagProperty, _appContext.ColumnDefinitionRepo.TypeToColumnMapper[type.Key]);

                var imageClickHandler = new MouseButtonEventHandler(Cell_Click);
                imageFactory.AddHandler(Image.MouseLeftButtonDownEvent, imageClickHandler);

                var imageRightClickHandler = new MouseButtonEventHandler(RightButtonCell_Click);
                imageFactory.AddHandler(Image.MouseRightButtonDownEvent, imageRightClickHandler);

                var dataTemplate = new DataTemplate(typeof(Image));
                dataTemplate.VisualTree = imageFactory;

                GridViewColumn column = new GridViewColumn();
                column.HeaderContainerStyle = (Style)TreeListControl.TryFindResource("HeaderContainerStyle");
                column.Width = 100.0;
                _remainingWidth += 100;

                // header template
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                var columnDefinition = _appContext.ColumnDefinitionRepo.ColumnDefinitions[type.Key];
                var header = Translations.Instance.Translate(columnDefinition.ResourceKey);
                textBlockFactory.SetValue(TextBlock.TextProperty, header);
                textBlockFactory.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.White);
                textBlockFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow);
                textBlockFactory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                textBlockFactory.SetValue(TextBlock.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight);
                textBlockFactory.SetValue(TextBlock.LineHeightProperty, 17.0);
                textBlockFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                textBlockFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                textBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 5, 0));
                textBlockFactory.SetValue(TextBlock.FontSizeProperty, 14.0);
                textBlockFactory.SetValue(TextBlock.WidthProperty, 100.0);

                var headerTemplate = new DataTemplate(typeof(TextBlock));
                headerTemplate.VisualTree = textBlockFactory;
                column.HeaderTemplate = headerTemplate;

                column.CellTemplate = dataTemplate;
                gridView.Columns.Add(column);
            }

            column1.Width = double.NaN - _remainingWidth;

            TreeListControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            TreeListControl.View = gridView;
        }

        private void TreeListControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender != null
                && sender is ListView listView
                && listView.View is GridView gridView)
            {
                if (listView.ActualWidth - _remainingWidth < 350)
                {
                    gridView.Columns[0].Width = 350;
                }
                else
                {
                    gridView.Columns[0].Width = listView.ActualWidth - _remainingWidth;
                }
            }
        }
    }
}