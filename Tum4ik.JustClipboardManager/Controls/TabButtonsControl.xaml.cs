using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for TabButtonsControl.xaml
/// </summary>
[ContentProperty(nameof(Tabs))]
public partial class TabButtonsControl
{
  public TabButtonsControl()
  {
    InitializeComponent();
  }


  public Collection<TabButton> Tabs { get; } = new();


  private void Root_Loaded(object sender, RoutedEventArgs e)
  {
    var groupName = Guid.NewGuid().ToString();
    foreach (var tab in Tabs)
    {
      tab.GroupName = groupName;
      tab.PreviewMouseLeftButtonDown += Tab_PreviewMouseLeftButtonDown;
      tab.Checked += Tab_Checked;

      if (tab.IsChecked is true)
      {
        Tab_Checked(tab, e);
        var command = tab.Command;
        var commandParameter = tab.CommandParameter;
        if (command is not null && command.CanExecute(commandParameter))
        {
          command.Execute(commandParameter);
        }
      }
    }
  }
  

  private TabButton? _checkedTab;


  private void Tab_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    var tab = (TabButton) sender;
    tab.PreviewMouseLeftButtonUp += Tab_PreviewMouseLeftButtonUp;
    e.Handled = true;
  }


  private void Tab_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    var tab = (TabButton) sender;
    tab.PreviewMouseLeftButtonUp -= Tab_PreviewMouseLeftButtonUp;
    tab.IsChecked = true;
  }


  private void Tab_Checked(object tab, RoutedEventArgs e)
  {
    EnableFluidScroll();
    _checkedTab = (TabButton) tab;
    _checkedTab.UncheckedLeftSeparatorVisibility = Visibility.Collapsed;
    _checkedTab.UncheckedRightSeparatorVisibility = Visibility.Collapsed;
    _checkedTab.RenderTransform = null;
    _checkedTab.BringIntoView();

    var checkedTabIndex = Tabs.IndexOf(_checkedTab);
    for (var i = 0; i < checkedTabIndex; i++)
    {
      var beforeTab = Tabs[i];
      beforeTab.UncheckedLeftSeparatorVisibility = Visibility.Visible;
      beforeTab.UncheckedRightSeparatorVisibility = Visibility.Collapsed;
      beforeTab.RenderTransform = new TranslateTransform(4, 0);
    }
    for (var i = checkedTabIndex + 1; i < Tabs.Count; i++)
    {
      var afterTab = Tabs[i];
      afterTab.UncheckedLeftSeparatorVisibility = Visibility.Collapsed;
      afterTab.UncheckedRightSeparatorVisibility = Visibility.Visible;
      afterTab.RenderTransform = new TranslateTransform(-4, 0);
    }
  }


  private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
  {
    SetScrollButtonsVisibility(e);
    if (e.ViewportWidthChange < 0)
    {
      _checkedTab?.BringIntoView();
    }
    else if (e.ViewportWidthChange > 0)
    {
      DisableFluidScroll();
    }
  }


  [RelayCommand]
  private void ScrollLeft(double amount)
  {
    EnableFluidScroll();
    _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - amount);
  }


  [RelayCommand]
  private void ScrollRight(double amount)
  {
    EnableFluidScroll();
    _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset + amount);
  }


  private void SetScrollButtonsVisibility(ScrollChangedEventArgs e)
  {
    var scrollButtonsVisibility = Visibility.Collapsed;
    if (e.ViewportWidth < e.ExtentWidth)
    {
      scrollButtonsVisibility = Visibility.Visible;
    }

    _scrollTabLeftButton.Visibility = scrollButtonsVisibility;
    _scrollTabRightButton.Visibility = scrollButtonsVisibility;
  }


  private void EnableFluidScroll()
  {
    _fluidMoveBehavior.Duration = TimeSpan.FromMilliseconds(300);
  }

  private void DisableFluidScroll()
  {
    _fluidMoveBehavior.Duration = TimeSpan.Zero;
  }
}
