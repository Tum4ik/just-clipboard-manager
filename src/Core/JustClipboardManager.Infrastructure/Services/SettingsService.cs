using System.Text.Json;
using JustClipboardManager.Application.Services;
using JustClipboardManager.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace JustClipboardManager.Infrastructure.Services;

internal class SettingsService : ISettingsService
{
  private readonly SettingsFilesOptions _settingsFilesOptions;

  public SettingsService(IOptions<SettingsFilesOptions> settingsFilesOptions,
                         IOptions<ProductInfoOptions> productInfoOptions)
  {
    _settingsFilesOptions = settingsFilesOptions.Value;

    _settingsFolderPath = Path.Join(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      productInfoOptions.Value.Name
    );
  }

  private readonly string _settingsFolderPath;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  private volatile bool _isInitialized;


  public UserSettings UserSettings { get => GetSettings(field); private set; } = null!;


  public async Task LoadSettingsAsync()
  {
    if (_isInitialized)
    {
      return;
    }

    await _semaphore.WaitAsync();
    try
    {
      if (_isInitialized)
      {
        return;
      }

      UserSettings = await LoadSettingsFileAsync<UserSettings>(
        Path.Join(_settingsFolderPath, $"{_settingsFilesOptions.UserSettings}.json")
      );

      _isInitialized = true;
    }
    finally
    {
      _semaphore.Release();
    }
  }

  public async Task SaveChangesAsync()
  {
    await SaveSettingsToFileAsync(Path.Join(_settingsFolderPath, $"{_settingsFilesOptions.UserSettings}.json"), UserSettings);
  }


  private static async Task<T> LoadSettingsFileAsync<T>(string filePath) where T : new()
  {
    if (!File.Exists(filePath))
    {
      return new();
    }

    await using var fileStream = File.OpenRead(filePath);
    return await JsonSerializer.DeserializeAsync<T>(fileStream) ?? new();
  }


  private static async Task SaveSettingsToFileAsync<T>(string filePath, T settings)
  {
    await using var fileStream = File.OpenWrite(filePath);
    await JsonSerializer.SerializeAsync(fileStream, settings);
  }


  private T GetSettings<T>(T value)
  {
    if (!_isInitialized)
    {
      throw new InvalidOperationException(
        $"{nameof(SettingsService)} is not initialized, call {nameof(LoadSettingsAsync)} method."
      );
    }
    return value;
  }
}
