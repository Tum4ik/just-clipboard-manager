import { Injectable } from '@angular/core';
import { Octokit } from "@octokit/rest";
import { major } from 'semver';
import packageJson from '../../../../../package.json';
import { MonitoringService } from '../../../core/services/monitoring.service';

@Injectable({ providedIn: 'root' })
export class GithubService {
  constructor(private readonly monitoringService: MonitoringService) { }

  private readonly octokit = new Octokit();

  async getPluginsListAsBase64ContentAsync(): Promise<string | null> {
    try {
      const pdkVersion = packageJson.dependencies['just-clipboard-manager-pdk'];
      const response = await this.octokit.repos.getContent({
        owner: 'Tum4ik',
        repo: 'just-clipboard-manager-plugins',
        path: `release/${major(pdkVersion)}/list.json`,
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
