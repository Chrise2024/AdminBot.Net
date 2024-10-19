using System;
using Newtonsoft.Json.Linq;

namespace AdminBot.Net.Utils
{
    public struct ConfigSchematics(
        string HttpServerUrl,
        string HttpPostUrl,
        List<long> GroupId,
        List<long> Commander,
        List<string> DisabledCommand,
        string CommandPrefix
        )
    {
        public string HttpServerUrl = HttpServerUrl;

        public string HttpPostUrl = HttpPostUrl;

        public List<long> GroupId = GroupId;

        public List<long> Commander = Commander;

        public List<string> DisabledCommand = DisabledCommand;

        public string CommandPrefix = CommandPrefix;
    }

    public struct ArgSchematics(
        string Command,
        List<string> Param,
        long CallerUin,
        int CallerPermissionLevel,
        long GroupId,
        int MsgId,
        bool Status
        )
    {
        public string Command = Command;

        public List<string> Param = Param;

        public long CallerUin = CallerUin;

        public int CallerPermissionLevel = CallerPermissionLevel;

        public long GroupId = GroupId;

        public int MsgId = MsgId;

        public bool Status = Status;
    }

    public struct CQEntitySchematics(string CQType)
    {
        public string CQType = CQType;
        public Dictionary<string, string> Properties;
    }

    public struct HitokotoSchematics(int Id = 0)
    {
        public int? id = Id;
        public string uuid;
        public string hitokoto;
        public string type;
        public string from;
        public string from_who;
        public string creator;
        public int creator_uid;
        public int reviewer;
        public string commit_from;
        public string created_at;
        public int length;
    }

    public struct GroupMemberSchematics(long GroupId = 0)
    {
        public long? group_id = GroupId;
        public long? user_id;
        public string? nickname;
        public string? card;
        public string? sex;
        public int? age;
        public string? area;
        public int? join_time;
        public int? last_sent_time;
        public string? level;
        public string? role;
        public bool? unfriendly;
        public string? title;
        public int? title_expire_time;
        public bool? card_changeable;
    }

    public struct MsgSenderSchematics(
        long UserId = 0,
        string NickName = "",
        string Sex = "",
        int Age = 0
        )
    {
        public long? user_id = UserId;
        public string? nickname = NickName;
        public string? sex = Sex;
        public int? age = Age;
    }
    public struct MsgBodySchematics(int message_id = 0)
    {
        public long? time;
        public long? self_id;
        public string? post_type;
        public string? message_type;
        public string? sub_type;
        public int? message_id = message_id;
        public long? group_id;
        public long? user_id;
        public List<JObject>? message;
        public string? raw_message;
        public int? font;
        public MsgSenderSchematics? sender;
    }
    internal class Schematics
    {
    }
}
