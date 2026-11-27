namespace JustClipboardManager.Application.Services;

public interface IDatabaseMigrator
{
  Task MigrateAsync(CancellationToken cancellationToken = default);
}
