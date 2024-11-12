using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IClipRepository
{
  Task AddAsync(Clip clip);
  IAsyncEnumerable<Clip> GetAsync(int skip = 0, int take = int.MaxValue, string? search = null, IEnumerable<int>? idsToIgnore = null);
  Task<List<FormattedDataObject>> GetFormattedDataObjectsAsync(int clipId);
  Task UpdateAsync(Clip clip);
  Task DeleteAsync(Clip clip);
  Task DeleteBeforeDateAsync(DateTime date);
}
