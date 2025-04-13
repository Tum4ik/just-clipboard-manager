import { Injectable } from '@angular/core';
import { Octokit } from "@octokit/rest";
import { MonitoringService } from '../../../core/services/monitoring.service';

@Injectable({ providedIn: 'root' })
export class GithubService {
  constructor(private readonly monitoringService: MonitoringService) { }

  private readonly octokit = new Octokit();

  async getPluginsListAsBase64ContentAsync(): Promise<string | null> {
    try {
      const response = await this.octokit.repos.getContent({
        owner: 'Tum4ik',
        repo: 'just-clipboard-manager-plugins',
        path: 'plugins-list-4.0.json',
      });
      if (response.status !== 200) {
        this.monitoringService.warning(`Failed to fetch plugins list. Status code: ${response.status}`);
        return null;
      }

      return (response.data as any).content;
    } catch (error) {
      this.monitoringService.error('Failed to fetch plugins list', error);
      return null;
    }
  }
}
