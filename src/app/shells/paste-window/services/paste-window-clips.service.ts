import { moveItemInArray } from '@angular/cdk/drag-drop';
import { DOCUMENT, Inject, Injectable, signal } from '@angular/core';
import { Clip } from '@app/core/data/models/clip.model';
import { ClipsRepository } from '@app/core/data/repositories/clips.repository';
import { PinnedClipsRepository } from '@app/core/data/repositories/pinned-clips.repository';
import { PluginsService } from '@app/core/services/plugins.service';
import { BehaviorSubject, debounce, distinctUntilChanged, interval } from 'rxjs';
import { PasteDataService } from './paste-data.service';

@Injectable()
export class PasteWindowClipsService {
  constructor(
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly pluginsService: PluginsService,
    private readonly pasteDataService: PasteDataService,
  ) {
    this.searchSubject.pipe(
      distinctUntilChanged((prev, curr) => prev.text === curr.text),
      debounce(search => interval(search.shouldDebounce ? 1000 : 0)),
    ).subscribe(search => {
      this.loadClipsFromScratchAsync();
    });
  }

  private readonly pinnedClipsRepository = new PinnedClipsRepository();
  private readonly clipsRepository = new ClipsRepository();

  private readonly searchSubject = new BehaviorSubject<{ text: string; shouldDebounce: boolean; }>(
    { text: '', shouldDebounce: true }
  );

  private readonly clipHtmlElements = new Map<number, HTMLElement>();

  readonly orderedPinnedClips = signal<PasteWindowClip[]>([]);
  readonly regularClips = signal<PasteWindowClip[]>([]);


  async loadPinnedClipsAsync() {
    const pinnedClips = await this.pinnedClipsRepository.getPinnedClipsAsync();
    const pinnedClipsMap = new Map(pinnedClips.map(pc => [pc.id, pc]));
    const nextIds = pinnedClips.map(pc => pc.orderNextId);
    const firstClip = pinnedClips.find(pc => !nextIds.includes(pc.id));

    let orderedClips: PasteWindowClip[] = [];

    if (firstClip) {
      // normal ordered loading
      let nextClip = firstClip;
      while (true) {
        const htmlElement = this.getClipHtmlElement(nextClip.clip);
        if (htmlElement) {
          orderedClips.push({ clipId: nextClip.id, htmlElement });
        }

        if (!nextClip.orderNextId || !pinnedClipsMap.has(nextClip.orderNextId)) {
          break;
        }
        nextClip = pinnedClipsMap.get(nextClip.orderNextId)!;
      }
    }

    if (!firstClip || orderedClips.length !== pinnedClips.length) {
      // something is broken, create new order
      orderedClips = [];
      for (let i = 0; i < pinnedClips.length; i++) {
        const pinnedClip = pinnedClips[i];
        const htmlElement = this.getClipHtmlElement(pinnedClip.clip);
        if (htmlElement) {
          orderedClips.push({ clipId: pinnedClip.id, htmlElement });
        }

        await this.pinnedClipsRepository.beginTransactionAsync();
        if (i > 0) {
          const prevPinnedClip = pinnedClips[i - 1];
          prevPinnedClip.orderNextId = pinnedClip.id;
          await this.pinnedClipsRepository.updatePinnedClipOrderNextIdAsync(prevPinnedClip.id, prevPinnedClip.orderNextId);
        }
        if (i === pinnedClips.length - 1) {
          pinnedClip.orderNextId = null;
          await this.pinnedClipsRepository.updatePinnedClipOrderNextIdAsync(pinnedClip.id, pinnedClip.orderNextId);
        }
        await this.pinnedClipsRepository.commitAsync();
      }
    }

    this.orderedPinnedClips.set(orderedClips);
  }


  async loadClipsFromScratchAsync() {
    // todo: adjust items count to the height of the paste window
    const clips = await this.loadClipsAsync({ skip: 0, take: 15 });
    this.regularClips.set(clips);
  }


  async loadMoreClipsAsync(amount: number) {
    const clipsToAdd = await this.loadClipsAsync({ skip: this.regularClips().length, take: amount });
    this.regularClips.update(clips => {
      clips.push(...clipsToAdd);
      return clips;
    });
  }


  filter(search: string) {
    this.searchSubject.next({ text: search, shouldDebounce: !!search });
  }


  async pasteClipAsync(clipId: number) {
    await this.pasteDataService.pasteDataAsync(clipId);
    await this.clipsRepository.updateClippedAtAsync(clipId, new Date());

    const currIndex = this.regularClips().findIndex(c => c.clipId === clipId);
    if (currIndex > 0) {
      this.regularClips.update(rc => {
        moveItemInArray(rc, currIndex, 0);
        return rc;
      });
    }
  }


  async pinClipAsync(clipId: number) {
    const lastPinnedClip = this.orderedPinnedClips().at(-1);
    await this.pinnedClipsRepository.addPinnedClipAsync(clipId);
    if (lastPinnedClip) {
      await this.pinnedClipsRepository.updatePinnedClipOrderNextIdAsync(lastPinnedClip.clipId, clipId);
    }

    this.regularClips.update(rc => {
      const clipToPin = rc.find(c => c.clipId === clipId)!;
      const index = rc.indexOf(clipToPin);
      rc.splice(index, 1);

      this.orderedPinnedClips.update(opc => {
        opc.push(clipToPin);
        return opc;
      });

      return rc;
    });
  }


  async unpinClipAsync(clipId: number) {
    const prevPinnedClipId = await this.pinnedClipsRepository.getPrevPinnedClipIdAsync(clipId);
    const nextPinnedClipId = await this.pinnedClipsRepository.getNextPinnedClipIdAsync(clipId);
    if (prevPinnedClipId) {
      await this.pinnedClipsRepository.updatePinnedClipOrderNextIdAsync(prevPinnedClipId, nextPinnedClipId);
    }
    await this.pinnedClipsRepository.deletePinnedClipAsync(clipId);
    await this.clipsRepository.updateClippedAtAsync(clipId, new Date());

    this.orderedPinnedClips.update(opc => {
      const clipToUnpin = opc.find(c => c.clipId === clipId)!;
      const index = opc.indexOf(clipToUnpin);
      opc.splice(index, 1);

      this.regularClips.update(rc => {
        rc.unshift(clipToUnpin);
        return rc;
      });

      return opc;
    });
  }


  async deleteClipAsync(clipId: number) {
    this.clipHtmlElements.delete(clipId);
    await this.clipsRepository.deleteClipAsync(clipId);
    const clipsToAdd = await this.loadClipsAsync({ skip: this.regularClips().length, take: 1 });
    this.regularClips.update(rc => {
      const currIndex = rc.findIndex(c => c.clipId === clipId);
      if (currIndex >= 0) {
        rc.splice(currIndex, 1);
      }
      rc.push(...clipsToAdd);
      return rc;
    });
  }


  private async loadClipsAsync(options: { skip: number; take: number; }): Promise<PasteWindowClip[]> {
    const enabledPluginIds = this.pluginsService.enabledPlugins.map(ep => ep.id);
    const clips = await this.clipsRepository.getClipsAsync(
      enabledPluginIds, options.skip, options.take, this.searchSubject.value.text
    );
    const clipsToAdd: PasteWindowClip[] = [];
    if (clips.length > 0) {

      for (const clip of clips) {
        const htmlElement = this.getClipHtmlElement(clip);
        if (!htmlElement) {
          continue;
        }
        clipsToAdd.push({ clipId: clip.id!, htmlElement });
      }
    }

    return clipsToAdd;
  }


  private getClipHtmlElement(clip: Clip): HTMLElement | null {
    if (this.clipHtmlElements.has(clip.id!)) {
      return this.clipHtmlElements.get(clip.id!)!;
    }

    const { plugin, isEnabled } = this.pluginsService.getPlugin(clip.pluginId) ?? {};
    if (!plugin || !isEnabled?.()) {
      return null;
    }
    const item = plugin.getRepresentationDataElement(
      { data: clip.representationData, metadata: clip.representationMetadata },
      clip.representationFormatName, this.document
    );
    this.clipHtmlElements.set(clip.id!, item);

    return item;
  }
}


export interface PasteWindowClip {
  clipId: number;
  htmlElement: HTMLElement;
}
