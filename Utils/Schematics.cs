using System;

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
    internal class Schematics
    {
    }
}
