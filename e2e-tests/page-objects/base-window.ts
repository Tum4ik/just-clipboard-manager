export abstract class BaseWindow {
  protected static async baseActivate(windowUrl: string): Promise<void> {
    if (await browser.getUrl() === windowUrl) {
      return;
    }

    const maxRetries = 5;
    const retryDelayMs = 500;
    const urls: string[] = [];

    for (let attempt = 0; attempt < maxRetries; attempt++) {
      const handles = await browser.getWindowHandles();

      for (const handle of handles) {
        await browser.switchToWindow(handle);
        const url = await browser.getUrl();
        urls.push(url);
        if (url === windowUrl) {
          return;
        }
      }

      if (attempt < maxRetries - 1) {
        await browser.pause(retryDelayMs);
      }
    }

    throw new Error(`Can't select window ${windowUrl}. Checked URLs: ${urls}`);
  }
}
