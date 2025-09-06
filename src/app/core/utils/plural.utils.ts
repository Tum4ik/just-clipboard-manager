/**
 * Returns the pluralization category for a given number and locale.
 * @param n The number to get the pluralization category for.
 * @param locale The locale to use for pluralization rules.
 * @returns The pluralization category (e.g., 'zero', 'one', 'two', 'few', 'many', 'other').
 */
export function getPluralCategory(n: number, locale: string): Intl.LDMLPluralRule {
  const pr = new Intl.PluralRules(locale);
  return pr.select(n);
}
