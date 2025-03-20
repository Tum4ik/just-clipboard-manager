export const LANGUAGE_INFO: { [key: string]: LanguageInfo; } = {
  'en': {
    nativeName: 'English (United States)'
  },
  'uk': {
    nativeName: 'українська (Україна)'
  }
};

export interface LanguageInfo {
  nativeName: string;
}
