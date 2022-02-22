namespace Devops.Pages;

using System.Collections.Immutable;

public record RepoReleaseVersionsInfo(ImmutableSortedSet<Version> OrderedVersions, Version CurrentVersion, Version NextVersion);