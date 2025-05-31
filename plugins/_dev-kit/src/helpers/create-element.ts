export function createElement<K extends keyof HTMLElementTagNameMap>(
  document: Document,
  tagName: K,
  style?: CSSStyleDeclaration,
  children?: Node[],
  options?: ElementCreationOptions
): HTMLElementTagNameMap[K] {
  const el = document.createElement(tagName, options);
  if (style) {
    for (const key in style) {
      el.style[key] = style[key];
    }
  }
  if (children) {
    for (const child of children) {
      el.appendChild(child);
    }
  }
  return el;
}
