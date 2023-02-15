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
      tab.MouseEnter += Tab_MouseEnter;

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
    _checkedTab.MouseLeave -= Tab_MouseLeave;
    _checkedTab.UncheckedLeftSeparatorVisibility = Visibility.Collapsed;
    _checkedTab.UncheckedRightSeparatorVisibility = Visibility.Collapsed;
    _checkedTab.RenderTransform = null;
    _checkedTab.BringIntoView();

    var checkedTabIndex = Tabs.IndexOf(_checkedTab);
    for (var i = 0; i < checkedTabIndex; i++)
    {
      var beforeTab = Tabs[i];
      beforeTab.MouseLeave -= Tab_MouseLeave;
      beforeTab.UncheckedLeftSeparatorVisibility = Visibility.Visible;
      beforeTab.UncheckedRightSeparatorVisibility = Visibility.Collapsed;
      beforeTab.RenderTransform = new TranslateTransform(4, 0);
    }
    for (var i = checkedTabIndex + 1; i < Tabs.Count; i++)
    {
      var afterTab = Tabs[i];
      afterTab.MouseLeave -= Tab_MouseLeave;
      afterTab.UncheckedLeftSeparatorVisibility = Visibility.Collapsed;
      afterTab.UncheckedRightSeparatorVisibility = Visibility.Visible;
      afterTab.RenderTransform = new TranslateTransform(-4, 0);
    }
  }


  private void Tab_MouseEnter(object sender, MouseEventArgs e)
  {
    var tab = (TabButton) sender;
    if (tab.IsChecked is true)
    {
      return;
    }

    var tabIndex = Tabs.IndexOf(tab);
    HideSeparators(tabIndex, HideType.Both, tab);
    HideSeparators(tabIndex - 1, HideType.Right);
    HideSeparators(tabIndex + 1, HideType.Left);
    tab.MouseLeave += Tab_MouseLeave;
  }


  private void Tab_MouseLeave(object sender, MouseEventArgs e)
  {
    var tab = (TabButton) sender;
    tab.MouseLeave -= Tab_MouseLeave;
    if (tab.IsChecked is true)
    {
      return;
    }

    RestoreSeparators();
  }


  private readonly Dictionary<int, (Visibility left, Visibility right)> _tabIndexToSeparatorsVisibilities = new();

  private void HideSeparators(int tabIndex, HideType hideType, TabButton? tab = null)
  {
    if (tabIndex < 0 || tabIndex >= Tabs.Count)
    {
      return;
    }

    tab ??= Tabs[tabIndex];
    if (tab.IsChecked is true)
    {
      return;
    }

    _tabIndexToSeparatorsVisibilities[tabIndex]
      = (tab.UncheckedLeftSeparatorVisibility, tab.UncheckedRightSeparatorVisibility);
    if (hideType == HideType.Left || hideType == HideType.Both)
    {
      tab.UncheckedLeftSeparatorVisibility = Visibility.Hidden;
    }
    if (hideType == HideType.Right || hideType == HideType.Both)
    {
      tab.UncheckedRightSeparatorVisibility = Visibility.Hidden;
    }
  }


  private enum HideType
  {
    Both, Left, Right
  }


  private void RestoreSeparators()
  {
    foreach (var (tabIndex, (left, right)) in _tabIndexToSeparatorsVisibilities)
    {
      var tab = Tabs[tabIndex];
      if (tab.IsChecked is true)
      {
        continue;
      }
      tab.UncheckedLeftSeparatorVisibility = left;
      tab.UncheckedRightSeparatorVisibility = right;
    }
    _tabIndexToSeparatorsVisibilities.Clear();
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
