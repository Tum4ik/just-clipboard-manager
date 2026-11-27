using JustClipboardManager.Application.Common.Abstractions;
using JustClipboardManager.Domain.Entities;
using JustClipboardManager.PluginDevKit.Core;
using Microsoft.EntityFrameworkCore;

namespace JustClipboardManager.Application.Services;

public interface IClipsService
{
  Task CreateClipAsync(ProcessDataResult processDataResult, CancellationToken cancellationToken = default);
}


internal class ClipsService : IClipsService
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public ClipsService(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }


  public async Task CreateClipAsync(ProcessDataResult processDataResult, CancellationToken cancellationToken = default)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
    var clip = new Clip
    {
      PreviewData = processDataResult.PreviewData,
      SearchLabel = processDataResult.SearchLabel,
    };
    foreach (var clipDataObject in processDataResult.FormattedDataResults)
    {
      clip.ClipDataObjects.Add(new ClipData
      {
        FormatId = clipDataObject.FormatId,
        Data = clipDataObject.Data
      });
    }
    await dbContext.Clips.AddAsync(clip, cancellationToken);
    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
