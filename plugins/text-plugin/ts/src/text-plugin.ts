export class TextPlugin /* extends ClipboardDataPlugin */ {
  /* override */ get id(): `${string}-${string}-${string}-${string}-${string}` {
    return 'd930d2cd-3fd9-4012-a363-120676e22afa';
  }

  /* override */ get formats(): readonly string[] {
    return ['text/plain'];
  }

  /* override */ extractRepresentationData(data: Buffer): Buffer {
    throw new Error("Method not implemented.");
  }

  /* override */ getRepresentationDataElement(data: Buffer): HTMLElement {
    const div = document.createElement('div');
    div.textContent = 'test clip';
    return div;
  }
}
