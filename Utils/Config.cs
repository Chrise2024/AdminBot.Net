using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using AdminBot.Net.Utils;

namespace AdminBot.Net.Utils
{
    internal class ConfigManager
    {
        private readonly List<string> DefaultCommands = [
            "help",
            "symmet",
            "titleself",
            "permission",
            "listop",
            "ban",
            "kick",
            "recall",
            "settitle",
            "op",
            "deop",
            "admin",
            "deadmin",
            "enable",
            "disable"
        ];
        private readonly JSchema ConfigJsonSchema = JSchema.Parse(
            @"
            {
            ""type"": ""object"",
            ""properties"": {
                    ""HttpServerUrl"": { ""type"": ""string"" , ""format"": ""uri"" },
                    ""HttpPostUrl"": { ""type"": ""string"" , ""format"": ""uri"" },
                    ""GroupId"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" } },
                    ""Commander"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" } },
                    ""DisabledCommand"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } },
                    ""CommandPrefix"": { ""type"": ""string"" }
                },
                ""required"": [""HttpServerUrl"", ""HttpPostUrl"", ""GroupId"", ""Commander"", ""DisabledCommand"", ""CommandPrefix""]
            }"
            );

        private readonly string ConfigPath = Path.Join(Program.GetProgramRoot(), "config.json");

        private ConfigSchematics Config;

        private readonly ConfigSchematics DefaultConfig = new("http://127.0.0.1:8089/", "http://127.0.0.1:8088", [], [], [],"/");

        /*
         * 8089为Bot上报消息的Url，即当前程序开启的Http Server地址
         * 8088为Bot接收Http请求的Url，即当前程序发送Http请求的地址
         */

        public ConfigManager()
        {
            if (!File.Exists(ConfigPath))
            {
                FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, DefaultConfig);
                Config = DefaultConfig;
            }
            else
            {
                JObject RConfig = FileIO.ReadAsJSON(ConfigPath);
                if (RConfig.IsValid(ConfigJsonSchema))
                {
                    Config = RConfig.ToObject<ConfigSchematics>();
                }
                else
                {
                    Config = DefaultConfig;
                    FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, DefaultConfig);
                }
            }
        }
        public string GetHttpServerUrl()
        {
            return Config.HttpServerUrl;
        }
        public string GetHttpPostUrl()
        {
            return Config.HttpPostUrl;
        }
        public List<long> GetGroupList()
        {
            return Config.GroupId;
        }
        public List<string> GetCommandList()
        {
            return DefaultCommands;
        }
        public List<long> GetCommanderList()
        {
            return Config.Commander;
        }

        public List<string> GetDisabledCommand()
        {
            return Config.DisabledCommand;
        }
        public string GetCommandPrefix()
        {
            return Config.CommandPrefix;
        }
        public int SetCommandStatus(string CommandName,bool Status)
        {
            if (DefaultCommands.Contains(CommandName))
            {
                if (Status)
                {
                    if (Config.DisabledCommand.Remove(CommandName))
                    {
                        SaveConfig();
                        return 200;
                    }
                    else
                    {
                        return 400;
                    }
                }
                else
                {
                    if (!Config.DisabledCommand.Contains(CommandName))
                    {
                        Config.DisabledCommand.Add(CommandName);
                        SaveConfig();
                        return 200;
                    }
                    else
                    {
                        return 400;
                    }
                }
            }
            else
            {
                return 404;
            }
        }
        private void SaveConfig()
        {
            FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, Config);
        }
    }

    internal class OPManager
    {

        private readonly string OPListPath = Path.Join(Program.GetProgramRoot(), "OPList.json");

        private Dictionary<long, List<long>> OPList = [];

        public OPManager()
        {
            if (!File.Exists(OPListPath))
            {
                FileIO.WriteAsJSON(OPListPath, []);
                OPList = [];
            }
            else
            {
                
                try
                {
                    OPList = FileIO.ReadAsJSON<Dictionary<long, List<long>>>(OPListPath);
                }
                catch
                {
                    FileIO.WriteAsJSON(OPListPath, []);
                    OPList = [];
                }
            }
        }
        public List<long> GetOPList(long TargetGroupId)
        {
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                return list;
            }
            else
            {
                return [];
            }
        }
        public int AddOP(long TargetGroupId, long TargetUin)
        {
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                if (!list.Contains(TargetUin))
                {
                    list.Add(TargetUin);
                    SaveOPList();
                    return 200;
                }
                else
                {
                    return 418;
                }
            }
            else
            {
                OPList.Add(TargetGroupId, [TargetUin]);
                return 200;
            }
        }
        public int AddOP(long TargetGroupId, string StrTargetUin)
        {
            long TargetUin = Int64.Parse(StrTargetUin);
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                if (!list.Contains(TargetUin))
                {
                    list.Add(TargetUin);
                    SaveOPList();
                    return 200;
                }
                else
                {
                    return 418;
                }
            }
            else
            {
                OPList.Add(TargetGroupId, [TargetUin]);
                return 200;
            }
        }
        public int RemoveOP(long TargetGroupId, long TargetUin)
        {
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                if (list.Remove(TargetUin))
                {
                    SaveOPList();
                    return 200;
                }
                else
                {
                    return 400;
                }
            }
            else
            {
                return 404;
            }
        }
        public int RemoveOP(long TargetGroupId, string StrTargetUin)
        {
            long TargetUin = Int64.Parse(StrTargetUin);
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                if (list.Remove(TargetUin))
                {
                    SaveOPList();
                    return 200;
                }
                else
                {
                    return 400;
                }
            }
            else
            {
                return 404;
            }
        }
        public bool IsOP(long TargetGroupId, long TargetUin)
        {
            if (OPList.TryGetValue(TargetGroupId,out var list))
            {
                return list.Contains(TargetUin);
            }
            else
            {
                return false;
            }
        }
        public bool IsOP(long TargetGroupId, string StrTargetUin)
        {
            long TargetUin = Int64.Parse(StrTargetUin);
            if (OPList.TryGetValue(TargetGroupId, out var list))
            {
                return list.Contains(TargetUin);
            }
            else
            {
                return false;
            }
        }
        private void SaveOPList()
        {
            FileIO.WriteAsJSON<Dictionary<long, List<long>>>(OPListPath, OPList);
        }
    }
}