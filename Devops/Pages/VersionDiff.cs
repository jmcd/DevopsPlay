namespace Devops.Pages;

public readonly record struct VersionDiff(int Major, int Minor, int Build, int Revision)
{
    public static VersionDiff From(Version a, Version b) =>
        new(
            a.Major - b.Major,
            a.Minor - b.Minor,
            a.Build - b.Build,
            a.Revision - b.Revision);

    public Version AddTo(Version v)
    {
        int Add(int v, int d)
        {
            if (v == -1 && d == 0) { return -1; }
            return v + d;
        }

        return new(
            Add(v.Major, Major),
            Add(v.Minor, Minor),
            Add(v.Build, Build),
            Add(v.Revision, Revision)
        );
    }
}