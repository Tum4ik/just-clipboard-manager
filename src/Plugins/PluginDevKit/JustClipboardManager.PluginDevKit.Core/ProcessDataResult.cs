namespace JustClipboardManager.PluginDevKit.Core;

public class ProcessDataResult
{
  public required IReadOnlyCollection<FormattedDataResult> FormattedDataResults { get; init; }
  public required byte[] PreviewData { get; init; }
  public string? SearchLabel { get; init; }
}


public class FormattedDataResult
{
  public string? FormatId { get; init; }
  public required byte[] Data { get; init; }
}
