using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IClipRepository
{
  Task AddAsync(Clip clip);
  IAsyncEnumerable<Clip> GetAsync(int skip = 0, int take = int.MaxValue, string? search = null);
  Task DeleteBeforeDateAsync(DateTime date);
}
