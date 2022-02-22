namespace Devops.Pages;

internal static class VersionConstruction
{
    private static int NonNeg(int i) => i < 0 ? 0 : i;

    public static Version NextMajor(this Version version) =>
        Construct(
            NonNeg(version.Major) + 1,
            version.Minor,
            version.Build,
            version.Revision);

    public static Version NextMinor(this Version version) =>
        Construct(
            version.Major,
            NonNeg(version.Minor) + 1,
            version.Build,
            version.Revision);

    private static Version Construct(int major, int minor, int build, int revision) =>
        build == -1 && revision == -1 ? new(major, minor)
        : revision == -1 ? new(major, minor, build)
        : new(major, minor, build, revision);
}