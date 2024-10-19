using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net.NetWork;
using AdminBot.Net.Utils;

namespace AdminBot.Net.Command
{
    internal class CommandResolver
    {
        private static readonly ArgSchematics InvalidCommandArg = new("null", [], 0, -1, 0, 0, false);

        private static readonly ArgSchematics NoneCommandArg = new("", [],0,-1,0,0, false);

        private static readonly List<long> WorkGRoup = Program.GetConfigManager().GetGroupList();

        private static readonly string CommandPrefix = Program.GetConfigManager().GetCommandPrefix();

        private static readonly Logger ArgLogger = new("CommandResolver", "ArgParse");

        private static readonly Logger HandleLogger = new("CommandResolver", "HandleMsg");

        private static readonly PermissionManager CRPermissionManager = Program.GetPermissionManager();
        private static async Task<ArgSchematics> Parse(MsgBodySchematics MsgBody)
        {
            long GroupId = MsgBody.group_id ?? 0;
            long CallerUin = MsgBody.user_id ?? 0;
            int MsgId = MsgBody.message_id ?? 0;
            string CQString = MsgBody.raw_message ?? "";
            int CallerPermissionLevel = await CRPermissionManager.GetPermissionLevel(GroupId,CallerUin);
            if (MsgId == 0 || CallerPermissionLevel == -1)
            {
                ArgLogger.Error("Invalid Msg Body");
                return NoneCommandArg;
            }
            else if (CQString.Length == 0)
            {
                ArgLogger.Error("Raw Msg Is Null");
                return NoneCommandArg;
            }
            else
            {
                ArgLogger.Info("Resolving, Raw = " + CQString);
                Match MatchCQReply = RegexProvider.GetCQReplyRegex().Match(CQString);
                if (MatchCQReply.Success)
                {
                    CQEntitySchematics CQEntity = DecodeCQEntity(MatchCQReply.Value);
                    int TargetMsgId = Int32.Parse(CQEntity.Properties.TryGetValue("id", out var IntMsgId) ? IntMsgId : "0");
                    string NormalCQString = CQString.Replace(MatchCQReply.Value, "").Trim();
                    if ( NormalCQString.StartsWith(CommandPrefix))
                    {
                        List<string> Params = ParseCQString(NormalCQString[CommandPrefix.Length..]);
                        return new ArgSchematics(
                            Params[0],
                            [$"{TargetMsgId}", ..Params[1..]],
                            CallerUin,
                            CallerPermissionLevel,
                            GroupId,
                            MsgId,
                            true
                            );
                    }
                }
                else if (CQString.StartsWith(CommandPrefix) && !CQString.Equals(CommandPrefix))
                {
                    List<string> Params = ParseCQString(CQString[CommandPrefix.Length..]);
                    return new ArgSchematics(
                            Params[0],
                            Params[1..],
                            CallerUin,
                            CallerPermissionLevel,
                            GroupId,
                            MsgId,
                            true
                            );
                }
            }
            return NoneCommandArg;
        }

        private static List<string> ParseCQString(string CQString)
        {
            if (CQString.Length == 0)
            {
                return [];
            }
            return new(
                RegexProvider.GetCQEntityRegex().Replace(
                    CQString, match => {
                        CQEntitySchematics CQEntity = DecodeCQEntity(match.Value);
                        if (CQEntity.CQType.Equals("at"))
                        {
                            if (CQEntity.Properties.TryGetValue("qq",out var Uin))
                            {
                                return $" {Uin} ";
                            }
                        }
                        else if (CQEntity.CQType.Equals("reply"))
                        {
                            if (CQEntity.Properties.TryGetValue("id", out var MsgId))
                            {
                                return $" {MsgId} ";
                            }
                        }
                        else if (CQEntity.CQType.Equals("image"))
                        {
                            if (CQEntity.Properties.TryGetValue("file", out var ImageUrl))
                            {
                                return $" {ImageUrl} ";
                            }
                        }
                        return " ";
                    }//$" {RegexProvider.GetIdRegex().Match(match.Value).Value} "
                ).Split(" ", StringSplitOptions.RemoveEmptyEntries)
            );
        }
        private static CQEntitySchematics DecodeCQEntity(string CQEntityString)
        {
            Dictionary<string,string> Properties = [];
            CQEntityString = CQEntityString[1..(CQEntityString.Length - 1)]
                .Replace(",", "\r")
                .Replace("&amp;", "&")
                .Replace("&#91;", "[")
                .Replace("&#93;", "]")
                .Replace("&#44;", ",");
            string[] CQPiece = CQEntityString.Split("\r");
            CQEntitySchematics CQEntity = new(CQPiece[0][3..]);
            for (int i = 1; i < CQPiece.Length; i++)
            {
                string[] temp = CQPiece[i].Split("=",2,StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length > 1)
                {

                    Properties.Add(temp[0], temp[1]);
                }
            }
            CQEntity.Properties = Properties;
            return CQEntity;
        }

        public static string ExtractUrlFromMsg(MsgBodySchematics MsgBody)
        {
            if (MsgBody.message?.Count > 0)
            {
                List<JObject> MsgChain = MsgBody.message;
                if (MsgChain.Count > 0)
                {
                    JObject Msg = MsgChain[0];
                    if (Msg.Value<string>("type")?.Equals("image") ?? false)
                    {
                        if (Msg.TryGetValue("data", out var JT))
                        {
                            return JT.ToObject<JObject>()?.Value<string>("file") ?? "";
                        }
                    }
                }
            }
            return "";
        }
        public static async Task HandleMsg(MsgBodySchematics MsgBody)
        {
            if ((MsgBody.post_type?.Equals("message") ?? false) &&
                (MsgBody.message_type?.Equals("group") ?? false) &&
                WorkGRoup.Contains(MsgBody.group_id ?? 0)
                )
            {
                ArgSchematics Args = await Parse(MsgBody);
                if (Args.Status)
                {
                    CommandExecutor.Execute(Args);
                }
            }
        }
    }
}
