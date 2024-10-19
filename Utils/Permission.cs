using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AdminBot.Net.NetWork;

namespace AdminBot.Net.Utils
{
    internal class PermissionManager
    {
        private readonly List<long> CommanderList = Program.GetConfigManager().GetCommanderList();

        private readonly OPManager POPManager = Program.GetOPManager();
        public async Task<int> GetPermissionLevel(long TargetGroupId, long TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin == 0)
            {
                return -1;
            }
            GroupMemberSchematics Member = await HttpApi.GetGroupMember(TargetGroupId, TargetUin);
            if ((Member.GroupId ?? 0) != 0)
            {
                if (CommanderList.Contains(TargetUin)) return 3;
                else if (!(Member.Role?.Equals("member") ?? true)) return 2;
                else if (POPManager.IsOP(TargetGroupId, TargetUin)) return 1;
                else return 0;
            }
            else
            {
                return -1;
            }
        }
        public async Task<int> GetPermissionLevel(long TargetGroupId, string TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin.Length == 0)
            {
                return -1;
            }
            GroupMemberSchematics Member = await HttpApi.GetGroupMember(TargetGroupId, TargetUin);
            if ((Member.GroupId ?? 0) != 0 && Int64.TryParse(TargetUin, out var IntUin))
            {
                if (CommanderList.Contains(IntUin)) return 3;
                else if (!(Member.Role?.Equals("member") ?? true)) return 2;
                else if (POPManager.IsOP(TargetGroupId, IntUin)) return 1;
                else return 0;
            }
            else
            {
                return -1;
            }
        }
        public async Task<bool> CheckUin(long TargetGroupId, long TargetUin)
        {
            return ((await HttpApi.GetGroupMember(TargetGroupId, TargetUin)).GroupId ?? 0) != 0;
        }
    }
}
