namespace VRChatContentManager.Core.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class GitCommitHashAttribute(string commitHash) : Attribute
{
    public string CommitHash => commitHash;
}