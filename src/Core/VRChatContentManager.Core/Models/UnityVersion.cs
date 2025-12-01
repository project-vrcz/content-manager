using System.Text.RegularExpressions;

namespace VRChatContentManager.Core.Models;

// 2022.3.60f1 -> Major: 2022, Minor: 3, UpdateNumber: 60, ReleaseType: Final, ReleaseNumber: 1
public sealed partial record UnityVersion(
    int Major,
    int Minor,
    int UpdateNumber,
    UnityVersionReleaseType ReleaseType,
    int ReleaseNumber
) : IComparable<UnityVersion>
{
    [GeneratedRegex(
        @"^(?<Major>\d+)\.(?<Minor>\d+)\.(?<UpdateNumber>\d+)(?<ReleaseType>[a-z]+)(?<ReleaseTypeNumber>\d+)$")]
    private static partial Regex GetUnityVersionRegex();

    public static UnityVersion? TryParse(string raw)
    {
        var regex = GetUnityVersionRegex();
        var match = regex.Match(raw);

        if (!match.Success)
        {
            return null;
        }

        var major = int.Parse(match.Groups["Major"].Value);
        var minor = int.Parse(match.Groups["Minor"].Value);
        var updateNumber = int.Parse(match.Groups["UpdateNumber"].Value);
        var releaseTypeString = match.Groups["ReleaseType"].Value;
        var releaseTypeNumber = int.Parse(match.Groups["ReleaseTypeNumber"].Value);

        var releaseType = releaseTypeString switch
        {
            "a" => UnityVersionReleaseType.Alpha,
            "b" => UnityVersionReleaseType.Beta,
            "rc" => UnityVersionReleaseType.ReleaseCandidate,
            "f" => UnityVersionReleaseType.Final,
            "p" => UnityVersionReleaseType.Patch, // Only for 2017.x or older
            "x" => UnityVersionReleaseType.Experimental,
            _ => UnityVersionReleaseType.Unknown
        };

        return new UnityVersion(major, minor, updateNumber, releaseType, releaseTypeNumber);
    }

    public int CompareTo(UnityVersion? other)
    {
        if (other is null) return 1;

        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0) return majorComparison;

        var minorComparison = Minor.CompareTo(other.Minor);
        if (minorComparison != 0) return minorComparison;

        var updateNumberComparison = UpdateNumber.CompareTo(other.UpdateNumber);
        if (updateNumberComparison != 0) return updateNumberComparison;

        var releaseTypeComparison = ReleaseType.CompareTo(other.ReleaseType);
        if (releaseTypeComparison != 0) return releaseTypeComparison;

        return ReleaseNumber.CompareTo(other.ReleaseNumber);
    }
}

// https://learn.unity.com/tutorial/choose-the-right-unity-version
// https://unity.com/releases/editor/archive
// https://github.com/vrc-get/vrc-get/blob/beffbeae476a2f7be66f3667e52373dc3201d1ad/vrc-get-vpm/src/version/unity_version.rs#L11-L33
public enum UnityVersionReleaseType
{
    /// <summary>
    /// It seems Unity never actually used this in their official releases.
    /// </summary>
    Experimental,
    
    /// <summary>
    /// Only used for Unity 2017.x or older.
    /// </summary>
    Patch,
    Final,

    /// <summary>
    /// It seems Unity never actually used this in their official releases.
    /// </summary>
    ReleaseCandidate,

    Beta,
    Alpha,
    Unknown
}