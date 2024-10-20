using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal class ClipRepository : IClipRepository
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public ClipRepository(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }


  public async Task AddAsync(Clip clip)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    await dbContext.Clips.AddAsync(clip).ConfigureAwait(false);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  public async IAsyncEnumerable<Clip> GetAsync(int skip = 0,
                                               int take = int.MaxValue,
                                               string? search = null,
                                               IEnumerable<int>? idsToIgnore = null)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var clips = dbContext.Clips
      .Where(c => idsToIgnore == null || !idsToIgnore.Contains(c.Id))
      .Where(c =>
        string.IsNullOrEmpty(search)
        || (!string.IsNullOrEmpty(c.SearchLabel) && EF.Functions.Like(c.SearchLabel, $"%{search}%"))
      )
      .OrderByDescending(c => c.ClippedAt)
      .Skip(skip)
      .Take(take)
      .Include(c => c.FormattedDataObjects.OrderBy(fdo => fdo.FormatOrder))
      .AsAsyncEnumerable();
    await foreach (var clip in clips.ConfigureAwait(false))
    {
      yield return clip;
    }
  }


  public async Task UpdateAsync(Clip clip)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    dbContext.Clips.Update(clip);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  public async Task DeleteAsync(Clip clip)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    dbContext.Clips.Remove(clip);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  [SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.",
    Justification = "There is no user input."
  )]
  public async Task DeleteBeforeDateAsync(DateTime date)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var clipIdsToRemove = dbContext.Clips
      .Where(c => c.ClippedAt < date)
      .Select(c => c.Id);
    var clipIdsCommaSeparated = string.Join(",", clipIdsToRemove);
    await dbContext.Database
      .ExecuteSqlRawAsync(
        $"DELETE FROM {nameof(AppDbContext.Clips)} WHERE {nameof(Clip.Id)} IN ({clipIdsCommaSeparated})"
      )
      .ConfigureAwait(false);
  }
}
