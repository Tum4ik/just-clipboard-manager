import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClipsService {
  getClipsAsync(options: GetClipsOptions): Promise<HTMLElement[]>{
    return window.clipsServiceAPI.getClips(options.skip, options.take, options.search);
  }
}

export class GetClipsOptions {
  skip: number = 0;
  take: number = 15;
  search?: string;
}
