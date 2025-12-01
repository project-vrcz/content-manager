using System.Web;

namespace VRChatContentManager.Core.Extensions;

public static class UriStringExtenstion
{
    extension(string uri)
    {
        public string AttachQueryString(IReadOnlyDictionary<string, string> parms)
        {
            var query = HttpUtility.ParseQueryString("");

            foreach (var (key, value) in parms)
            {
                query[key] = value;
            }

            return uri.AttachQueryString(query.ToString() ?? string.Empty);
        }

        public string AttachQueryString(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return uri;

            return uri.Contains('?') ? $"{uri}&{queryString}" : $"{uri}?{queryString}";
        }
    }
}