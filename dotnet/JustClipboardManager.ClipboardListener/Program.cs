Gtk.Application.Init();
Gtk.Clipboard.Get(Gdk.Selection.Clipboard).OwnerChange += (sender, e) =>
{
  Console.WriteLine("clipboard-updated");
};
Gtk.Application.Run();
