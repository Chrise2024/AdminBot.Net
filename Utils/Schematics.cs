using System;

namespace AdminBot.Net.Utils
{
    public struct ConfigSchematics(
        string HttpServerUrl,
        string HttpPostUrl,
        List<string> GroupId,
        List<string> Commander,
        List<string> DisabledCommand,
        string CommandPrefix
        )
    {
        public string HttpServerUrl = HttpServerUrl;

        public string HttpPostUrl = HttpPostUrl;

        public List<string> GroupId = GroupId;

        public List<string> Commander = Commander;

        public List<string> DisabledCommand = DisabledCommand;

        public string CommandPrefix = CommandPrefix;
    }

    public struct ArgSchematics(
        string Command,
        List<string> Param,
        string CallerUin,
        int CallerPermissionLevel,
        string GroupId,
        string MsgId,
        bool Status//,
        //string TargetUin = "",
        //int TargetPermissionLevel = 0
        )
    {
        public string Command = Command;

        public List<string> Param = Param;

        public string CallerUin = CallerUin;

        public int CallerPermissionLevel = CallerPermissionLevel;

        //public string TargetUin = TargetUin;

        //public int TargetPermissionLevel = TargetPermissionLevel;

        public string GroupId = GroupId;

        public string MsgId = MsgId;

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
