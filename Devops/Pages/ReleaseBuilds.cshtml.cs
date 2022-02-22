namespace Devops.Pages;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

public class ReleaseBuilds : PageModel
{
    [BindProperty]
    [DataType(DataType.Password)]
    public string PersonalAccessToken { get; set; }

    [BindProperty]
    public string CollectionUri { get; set; }

    [BindProperty]
    public string ProjectName { get; set; }

    [BindProperty]
    public string RepoName { get; set; }


    public RepoReleaseVersionsInfo? RepoReleaseVersionsInfo { get; set; }

    public void OnGet() { }

    public async Task OnPostAsync()
    {
        var creds = new VssBasicCredential(string.Empty, PersonalAccessToken);

        var connection = new VssConnection(new(CollectionUri), creds);

        using var gitClient = connection.GetClient<GitHttpClient>();

        var repo = await gitClient.GetRepositoryAsync(ProjectName, RepoName);

        RepoReleaseVersionsInfo = await GetRepoInfo(gitClient, repo);
    }

    private static async Task<RepoReleaseVersionsInfo?> GetRepoInfo(GitHttpClient gitClient, GitRepository repo)
    {
        const string refNamePrefix = "refs/heads/release/";

        var orderedVersions = (await gitClient.GetBranchRefsAsync(repo.Id))
            .Where(@ref => @ref.Name.StartsWith(refNamePrefix))
            .Select(@ref => @ref.Name)
            .Select(n => n.Remove(0, refNamePrefix.Length))
            .Select(Version.Parse)
            .OrderBy(v => v.Major)
            .ThenBy(v => v.Minor)
            .ThenBy(v => v.Build)
            .ThenBy(v => v.Revision)
            .ToImmutableSortedSet();

        if (!orderedVersions.Any())
        {
            return null;
        }

        var major = 0;
        var minor = 0;
        for (var i = 1; i < orderedVersions.Count; i++)
        {
            var va = orderedVersions[i - 1];
            var vb = orderedVersions[i];
            major += vb.Major - va.Major;
            minor += vb.Minor - va.Minor;
        }

        var currentVersion = orderedVersions.Last();
        var nextVersion = major > minor ? currentVersion.NextMajor() : currentVersion.NextMinor();

        return new(orderedVersions, currentVersion, nextVersion);
    }
}