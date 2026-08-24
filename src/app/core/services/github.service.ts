import { inject, Service } from '@angular/core';
import { Octokit } from "@octokit/rest";
import packageJson from '../../../../package.json';
import { MonitoringService } from './monitoring.service';

const REPOSITORY_OWNER = 'Tum4ik';
const REPOSITORY_NAME = 'just-clipboard-manager-plugins';

@Service()
export class GithubService {
  private readonly monitoringService = inject(MonitoringService);


  private async getPluginNamesListAsync(): Promise<readonly string[]> {
    return ['files-plugin', /* 'images-plugin' */];
  }


  async getPluginsLatestReleasesTagNamesAsync(/* tagNamePrefix: string */): Promise<readonly string[]> {
    const pluginNamesList = await this.getPluginNamesListAsync();

    const res = await this.octokit.rest.git.listMatchingRefs({
      owner: REPOSITORY_OWNER,
      repo: REPOSITORY_NAME,
      ref: 'tags/files-plugin',

    });
    console.log(res);

    // const refsFragment = pluginNamesList.map((tagNamePrefix, i) => {
    //   return /* graphql */`
    //     plugin${i}: refs(refPrefix: "refs/tags/", first: 1, query: "${tagNamePrefix}", orderBy: {field: TAG_COMMIT_DATE, direction: DESC}) {
    //       nodes {
    //         name
    //       }
    //     }
    //     `;
    // }).join('\n');
    // // const { repository } = await graphql<{ repository: Pick<Repository, 'refs'>; }>(
    // const test = await this.octokit.graphql(
    //   /* graphql */`
    //   query {
    //     repository(owner: "${REPOSITORY_OWNER}", name: "${REPOSITORY_NAME}") {
    //       refs(refPrefix: "refs/tags/", first: 1, query: "files-plugin", orderBy: {field: TAG_COMMIT_DATE, direction: DESC}) {
    //       nodes {
    //         name
    //       }
    //     }
    //     }
    //   }
    //   `
    // );


    return [];
  }











  private readonly octokit = new Octokit();
  private readonly textDecoder = new TextDecoder();

  async getPluginsListAsync(): Promise<readonly string[] | null> {
    try {
      const pdkVersion = packageJson.dependencies['just-clipboard-manager-pdk'];

      const response = await this.octokit.repos.getContent({
        owner: 'Tum4ik',
        repo: 'just-clipboard-manager-plugins',
        path: `plugins-list`,
      });
      if (response.status !== 200) {
        this.monitoringService.warning(`Failed to fetch plugins list. Status code: ${response.status}`);
        return null;
      }

      const data = response.data;
      if (Array.isArray(data) || data.type !== 'file') {
        this.monitoringService.warning('Unexpected response type for plugins list');
        return null;
      }

      return atob(data.content).split(/\r?\n/).filter(line => !!line);
    } catch (error) {
      this.monitoringService.error('Failed to fetch plugins list', error);
      return null;
    }
  }
}
