using Microsoft.EntityFrameworkCore;
using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
// todo: unify common code with ClipRepository
internal class PinnedClipRepository : IPinnedClipRepository
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

  public PinnedClipRepository(IDbContextFactory<AppDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }


  public async Task AddAsync(PinnedClip clip)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    dbContext.Attach(clip.Clip);
    await dbContext.PinnedClips.AddAsync(clip).ConfigureAwait(false);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  public async IAsyncEnumerable<PinnedClip> GetAllOrderedAscAsync()
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var clips = dbContext.PinnedClips
      .OrderBy(c => c.Order)
      .Include(c => c.Clip).ThenInclude(c => c.FormattedDataObjects.OrderBy(fdo => fdo.FormatOrder))
      .AsAsyncEnumerable();
    await foreach (var clip in clips.ConfigureAwait(false))
    {
      yield return clip;
    }
  }


  public async Task UpdateAsync(PinnedClip clip)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    dbContext.PinnedClips.Update(clip);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }


  public async Task DeleteByIdAsync(int id)
  {
    using var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
    var clip = new PinnedClip { Id = id };
    dbContext.PinnedClips.Remove(clip);
    await dbContext.SaveChangesAsync().ConfigureAwait(false);
  }
}
