using System;
using System.Text.RegularExpressions;

namespace AdminBot.Net.Utils
{
    internal partial class RegexProvider
    {
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
        /*
        public static int IsValidCommand(string CQString)
        {
            if (!CQString.StartsWith(CommandPrefix))
            {
                if (CQString.Contains(CommandPrefix) && GetCQReplyRegex().Match(CQString).Success)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (GetCommandRegexTAX().Match(CQString).Success)
            {
                return 2;
            }
            else if (GetCommandRegexTTX().Match(CQString).Success)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
        */
        /*
         * UsedOn:
         * titleself
         * ban
         * kick
         * settitle
         * (de)op
         * (de)admin
         * enable
         * disable
         */
        /*
        [GeneratedRegex(@"^\w+\s*\w+\s*\w*.*(?<! )$")]
        public static partial Regex GetCommandRegexTTX();
         //Pattern:"^xxx xxx xxx" or "xxx xxx xxx"

        [GeneratedRegex(@"^\w+\s*\[CQ:at\S+,\S+\]\s*\w*.*(?<! )$")]
        public static partial Regex GetCommandRegexTAX();
        //Pattern:"^xxx [CQAt] xxx" or "xxx [CQCode]"

        [GeneratedRegex(@"^\w+.*(?<! )$")]
        public static partial Regex GetCommandRegexT();
        //Pattern:"^xxx"
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
