using Tum4ik.JustClipboardManager.Data.Models;

namespace Tum4ik.JustClipboardManager.Data.Repositories;
internal interface IPinnedClipRepository
{
  Task AddAsync(PinnedClip clip);
  IAsyncEnumerable<PinnedClip> GetAllOrderedAscAsync();
  Task UpdateAsync(PinnedClip clip);
  Task DeleteByIdAsync(int id);
}
