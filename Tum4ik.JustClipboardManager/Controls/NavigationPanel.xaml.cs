using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;

namespace Tum4ik.JustClipboardManager.Controls;

/// <summary>
/// Interaction logic for NavigationPanel.xaml
/// </summary>
[ContentProperty(nameof(Buttons))]
public partial class NavigationPanel
{
  public NavigationPanel()
  {
    InitializeComponent();
  }


  public Collection<NavigationButton> Buttons { get; } = new();


  private void Root_Loaded(object sender, RoutedEventArgs e)
  {
    var groupName = Guid.NewGuid().ToString();
    foreach (var button in Buttons)
    {
      button.GroupName = groupName;
      if (button.IsChecked is true)
      {
        var command = button.Command;
        var commandParameter = button.CommandParameter;
        if (command is not null && command.CanExecute(commandParameter))
        {
          command.Execute(commandParameter);
        }
      }
    }
  }
}
