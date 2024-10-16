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
        private static readonly ArgSchematics InvalidCommandArg = new("null", [], "", -1, "", "", false/*, "", -1*/);

        private static readonly ArgSchematics NoneCommandArg = new("", [],"",-1,"","", false/*, "",-1*/);

        private static readonly List<string> WorkGRoup = Program.GetConfigManager().GetGroupList();

        private static readonly string CommandPrefix = Program.GetConfigManager().GetCommandPrefix();

        private static readonly Logger ArgLogger = new("CommandResolver.ArgParse");

        private static readonly Logger HandleLogger = new("CommandResolver.HandleMsg");
        private static ArgSchematics Parse(JObject MsgBody)
        {
            string GroupId = MsgBody.Value<string>("group_id") ?? "";
            string CallerUin = MsgBody.Value<string>("user_id") ?? "";
            string MsgId = MsgBody.Value<string>("message_id") ?? "";
            string CQString = MsgBody.Value<string>("raw_message") ?? "";
            int CallerPermissionLevel = Program.GetPermissionManager().GetPermissionLevel(GroupId,CallerUin);
            if (MsgId.Length == 0 || CallerPermissionLevel == -1)
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
                    string TargetMsgId = RegexProvider.GetIdRegex().Match(MatchCQReply.Value).Value;
                    string NormalCQString = CQString.Replace(MatchCQReply.Value, "").Trim();
                    if ( NormalCQString.StartsWith(CommandPrefix))
                    {
                        List<string> Params = ParseCQString(NormalCQString[CommandPrefix.Length..]);
                        return new ArgSchematics(
                            Params[0],
                            [TargetMsgId, ..Params[1..]],
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
            Dictionary<string, string> Properties = [];
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

        public static string ExtractUrlFromMsg(JObject MsgBody)
        {
            if (MsgBody.TryGetValue("message",out var JA))
            {
                List<JObject> MsgChain = JA.ToObject<List<JObject>>() ?? [];
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
        public static void HandleMsg(JObject MsgBody)
        {
            if ((MsgBody.Value<string>("post_type")?.Equals("message") ?? false) &&
                (MsgBody.Value<string>("message_type")?.Equals("group") ?? false) &&
                (WorkGRoup.Contains(MsgBody.Value<string>("group_id") ?? ""))
                )
            {
                Console.WriteLine(JsonConvert.SerializeObject(MsgBody, Formatting.Indented));
                ArgSchematics Args = Parse(MsgBody);
                if (Args.Status)
                {
                    CommandExecutor.Execute(Args);
                }
            }
        }
    }
}
