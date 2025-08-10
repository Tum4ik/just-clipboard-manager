export function createElement<K extends keyof HTMLElementTagNameMap>(
  document: Document,
  tagName: K,
  options?: CreateElementOptions<K>
): HTMLElementTagNameMap[K] {
  const el = document.createElement(tagName, options);
  const props = options?.props;
  const style = options?.style;
  const children = options?.children;
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

export interface CreateElementOptions<K extends keyof HTMLElementTagNameMap> extends ElementCreationOptions {
  props?: HTMLElementTagNameMap[K] | Record<string, any>;
  style?: CSSStyleDeclaration | Record<string, any>;
  children?: Node[];
}
