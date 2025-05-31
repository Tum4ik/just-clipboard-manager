export function createElement<K extends keyof HTMLElementTagNameMap>(
  document: Document,
  tagName: K,
  props?: HTMLElementTagNameMap[K] | Record<string, any>,
  style?: CSSStyleDeclaration | Record<string, any>,
  children?: Node[],
  options?: ElementCreationOptions
): HTMLElementTagNameMap[K] {
  const el = document.createElement(tagName, options);
  if (props) {
    for (const key in props) {
      (el as any)[key] = (props as any)[key];
    }
  }
  if (style) {
    for (const key in style) {
      (el.style as any)[key] = (style as any)[key];
    }
  }
  if (children) {
    for (const child of children) {
      el.appendChild(child);
    }
  }
  return el;
}
