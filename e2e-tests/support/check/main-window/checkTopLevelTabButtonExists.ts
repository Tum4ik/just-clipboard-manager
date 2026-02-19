export default async function (tabId: string) {
  const selector = `[ngTabList] [data-testid="${tabId.toLowerCase()}"]`;
  const tab = $(selector);
  await expect(tab).toBeExisting();
}
