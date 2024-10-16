using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AdminBot.Net.NetWork;

namespace AdminBot.Net.Utils
{
    internal class PermissionManager
    {
        public int GetPermissionLevel(long TargetGroupId, long TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin == 0)
            {
                return -1;
            }
            Console.WriteLine(1);
            JObject member = HttpApi.GetGroupMember(TargetGroupId, TargetUin);
            if (member.Value<int>("group_id") > 0)
            {
                if (Program.GetConfigManager().GetCommanderList().Contains(TargetUin)) return 3;
                else if ((!(member.Value<string>("role")?.Equals("member"))) ?? false) return 2;
                else if (Program.GetOPManager().IsOP(TargetGroupId, TargetUin)) return 1;
                else return 0;
            }
            else
            {
                return -1;
            }
        }
        public int GetPermissionLevel(long TargetGroupId, string TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin.Length == 0)
            {
                return -1;
            }
            JObject member = HttpApi.GetGroupMember(TargetGroupId, TargetUin);
            long IntUin = Int64.Parse(TargetUin);
            if (member.Value<int>("group_id") > 0)
            {
                if (Program.GetConfigManager().GetCommanderList().Contains(IntUin)) return 3;
                else if ((!(member.Value<string>("role")?.Equals("member"))) ?? false) return 2;
                else if (Program.GetOPManager().IsOP(TargetGroupId, IntUin)) return 1;
                else return 0;
            }
            else
            {
                return -1;
            }
        }
        public bool CheckUin(long TargetGroupId, long TargetUin)
        {
            return HttpApi.GetGroupMember(TargetGroupId,TargetUin).Value<string>("group_id")?.Length > 0;
        }
    }
}
