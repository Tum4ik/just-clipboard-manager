using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
    _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
  }


  public Collection<TabButton> Tabs { get; } = new();



  private void Root_Loaded(object sender, RoutedEventArgs e)
  {
    var groupName = Guid.NewGuid().ToString();
    foreach (var tab in Tabs)
    {
      tab.GroupName = groupName;
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

  
  private void Tab_Checked(object tab, RoutedEventArgs e)
  {
    var checkedTab = (TabButton) tab;
    checkedTab.UncheckedLeftSeparatorVisibility = Visibility.Collapsed;
    checkedTab.UncheckedRightSeparatorVisibility = Visibility.Collapsed;
    checkedTab.RenderTransform = null;

    var checkedTabIndex = Tabs.IndexOf(checkedTab);
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


  private double _adjustedOffset;

  private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
  {
    var scrollButtonsVisibility = Visibility.Collapsed;
    if (e.ViewportWidth < e.ExtentWidth)
    {
      scrollButtonsVisibility = Visibility.Visible;
    }

    _scrollTabLeftButton.Visibility = scrollButtonsVisibility;
    _scrollTabRightButton.Visibility = scrollButtonsVisibility;

    if (e.HorizontalOffset == _adjustedOffset)
    {
      return;
    }

    if (e.HorizontalChange < 0)
    {
      _adjustedOffset = e.HorizontalOffset - 4;
    }
    else if (e.HorizontalChange > 0)
    {
      _adjustedOffset = e.HorizontalOffset + 4;
    }

    _scrollViewer.ScrollToHorizontalOffset(_adjustedOffset);
  }


  [RelayCommand]
  private void ScrollLeft(double amount)
  {
    _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - amount);
  }


  [RelayCommand]
  private void ScrollRight(double amount)
  {
    _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset + amount);
  }


  private static void ActivateTab(TabButton tab)
  {
    tab.IsChecked = true;
    tab.Focus();
  }
}
