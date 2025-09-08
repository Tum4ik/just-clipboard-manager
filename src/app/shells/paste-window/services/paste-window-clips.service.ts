import { moveItemInArray } from '@angular/cdk/drag-drop';
import { DOCUMENT, Inject, Injectable, signal } from '@angular/core';
import { Clip } from '@app/core/data/models/clip.model';
import { ClipsRepository } from '@app/core/data/repositories/clips.repository';
import { PinnedClipsRepository } from '@app/core/data/repositories/pinned-clips.repository';
import { PluginsService } from '@app/core/services/plugins.service';
import { SettingsService } from '@app/core/services/settings.service';
import { BehaviorSubject, debounce, distinctUntilChanged, interval } from 'rxjs';
import { PasteDataService } from './paste-data.service';

@Injectable()
export class PasteWindowClipsService {
  constructor(
    @Inject(DOCUMENT) private readonly document: Document,
    private readonly pluginsService: PluginsService,
    private readonly pasteDataService: PasteDataService,
    private readonly settingsService: SettingsService,
    private readonly pinnedClipsRepository: PinnedClipsRepository,
    private readonly clipsRepository: ClipsRepository,
  ) {
    this.searchSubject.pipe(
      distinctUntilChanged((prev, curr) => prev.text === curr.text),
      debounce(search => interval(search.shouldDebounce ? 1000 : 0)),
    ).subscribe(search => {
      this.loadClipsFromScratchAsync();
    });
  }

  private readonly searchSubject = new BehaviorSubject<{ text: string; shouldDebounce: boolean; }>(
    { text: '', shouldDebounce: true }
  );

  private readonly clipHtmlElements = new Map<number, HTMLElement>();

  private readonly _orderedPinnedClips = signal<PasteWindowClip[]>([]);
  readonly orderedPinnedClips = this._orderedPinnedClips.asReadonly();

  private readonly _regularClips = signal<PasteWindowClip[]>([]);
  readonly regularClips = this._regularClips.asReadonly();


  async loadPinnedClipsAsync() {
    const pinnedClips = await this.pinnedClipsRepository.getPinnedClipsAsync();
    const pinnedClipsMap = new Map(pinnedClips.map(pc => [pc.id, pc]));

    const orderedClips: PasteWindowClip[] = [];

    // Get pinned clips order from settings
    const order = await this.settingsService.getPinnedClipsOrderAsync();
    let shouldSaveNewOrder = false;

    for (const pinnedClipId of order) {
      if (pinnedClipsMap.has(pinnedClipId)) {
        const pinnedClip = pinnedClipsMap.get(pinnedClipId)!;
        const htmlElement = this.getClipHtmlElement(pinnedClip.clip);
        if (htmlElement) {
          orderedClips.push({ clipId: pinnedClip.id, htmlElement });
        }
        pinnedClipsMap.delete(pinnedClipId);
      }
      else {
        order.splice(order.indexOf(pinnedClipId), 1);
        shouldSaveNewOrder = true;
      }
    }

    if (pinnedClipsMap.size > 0) {
      for (const [id, pinnedClip] of pinnedClipsMap) {
        const htmlElement = this.getClipHtmlElement(pinnedClip.clip);
        if (htmlElement) {
          orderedClips.push({ clipId: pinnedClip.id, htmlElement });
        }
        order.push(id);
      }

      shouldSaveNewOrder = true;
    }

    if (shouldSaveNewOrder) {
      await this.settingsService.setPinnedClipsOrderAsync(order);
    }

    this._orderedPinnedClips.set(orderedClips);
  }


  async loadClipsFromScratchAsync() {
    // todo: adjust items count to the height of the paste window
    const clips = await this.loadClipsAsync({ skip: 0, take: 15 });
    this._regularClips.set(clips);
  }


  async loadMoreClipsAsync(amount: number) {
    const clipsToAdd = await this.loadClipsAsync({ skip: this.regularClips().length, take: amount });
    this._regularClips.update(clips => {
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
      this._regularClips.update(rc => {
        moveItemInArray(rc, currIndex, 0);
        return rc;
      });
    }
  }


  async pinClipAsync(clipId: number) {
    await this.pinnedClipsRepository.addPinnedClipAsync(clipId);
    const order = await this.settingsService.getPinnedClipsOrderAsync();
    order.push(clipId);
    await this.settingsService.setPinnedClipsOrderAsync(order);

    this._regularClips.update(rc => {
      const clipToPin = rc.find(c => c.clipId === clipId)!;
      const index = rc.indexOf(clipToPin);
      rc.splice(index, 1);

      this._orderedPinnedClips.update(opc => {
        opc.push(clipToPin);
        return opc;
      });

      return rc;
    });
  }


  async unpinClipAsync(clipId: number) {
    const order = await this.settingsService.getPinnedClipsOrderAsync();
    order.splice(order.indexOf(clipId), 1);
    await this.settingsService.setPinnedClipsOrderAsync(order);

    await this.pinnedClipsRepository.deletePinnedClipAsync(clipId);
    await this.clipsRepository.updateClippedAtAsync(clipId, new Date());

    this._orderedPinnedClips.update(opc => {
      const clipToUnpin = opc.find(c => c.clipId === clipId)!;
      const index = opc.indexOf(clipToUnpin);
      opc.splice(index, 1);

      this._regularClips.update(rc => {
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
    this._regularClips.update(rc => {
      const currIndex = rc.findIndex(c => c.clipId === clipId);
      if (currIndex >= 0) {
        rc.splice(currIndex, 1);
      }
      rc.push(...clipsToAdd);
      return rc;
    });
  }


  private async loadClipsAsync(options: { skip: number; take: number; }): Promise<PasteWindowClip[]> {
    const enabledPluginIds = this.pluginsService.enabledPlugins().map(ep => ep.id);
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
    const { plugin, isEnabled } = this.pluginsService.getPlugin(clip.pluginId) ?? {};
    if (!plugin || !isEnabled?.()) {
      return null;
    }

    if (this.clipHtmlElements.has(clip.id!)) {
      return this.clipHtmlElements.get(clip.id!)!;
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
