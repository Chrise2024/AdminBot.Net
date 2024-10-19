using System;
using System.Text.RegularExpressions;

namespace AdminBot.Net.Utils
{
    internal partial class RegexProvider
    {
        /*
        private static readonly string CommandPrefix = Program.GetConfigManager().GetCommandPrefix();
        public static string AtUinExtractor(string CQString)
        {
            Match m = GetCQAtRegex().Match(CQString);
            return m.Success ? GetIdRegex().Match(m.Value).Value : "";
        }
        public static string ReplyIdExtractor(string CQString)
        {
            Match m = GetCQReplyRegex().Match(CQString);
            return m.Success ? GetIdRegex().Match(m.Value).Value : "";
        }
        */

        [GeneratedRegex(@"\[CQ:\S+\]")]
        public static partial Regex GetCQEntityRegex();

        [GeneratedRegex(@"\[CQ:at,qq=\d+\S*\]")]
        public static partial Regex GetCQAtRegex();

        [GeneratedRegex(@"^\[CQ:reply,id=[-]*\d+\]")]
        public static partial Regex GetCQReplyRegex();

        [GeneratedRegex(@"\d+")]
        public static partial Regex GetIdRegex();
    }
}
