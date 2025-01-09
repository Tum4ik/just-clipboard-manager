export { };

declare global {
  interface Window {
    // clipsServiceAPI: {
    //   getClips(skip: number, take: number, search?: string): Promise<HTMLElement[]>;
    // }
    clipsServiceAPI: any;
  }
}
