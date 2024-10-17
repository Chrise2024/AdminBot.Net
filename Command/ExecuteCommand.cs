using System;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageMagick;
using ImageMagick.Drawing;
using AdminBot.Net.NetWork;
using AdminBot.Net.Utils;
using AdminBot.Net.Extra;

namespace AdminBot.Net.Command
{
    internal class CommandExecutor
    {
        private static readonly Logger ExecuteLogger = new("CommandExecutor.Execute");

        private static readonly List<string> AvaliableCommands = Program.GetConfigManager().GetCommandList();

        public static async void Execute(ArgSchematics Args)
        {
            if (AvaliableCommands.Contains(Args.Command))
            {
                bool Enabled = !Program.GetConfigManager().GetDisabledCommand().Contains(Args.Command);
                ExecuteLogger.Info($"Executing: <{Args.Command}>, Enabled: {Enabled},  With Arg\n{JsonConvert.SerializeObject(Args, Formatting.Indented)}");
                if (Enabled)
                {
                    int TargetPermissionLevel;
                    if (Args.Command.Equals("help"))
                    {
                        //ParamFormat: [<Command>]
                        if (Args.Param.Count > 0)
                        {
                            if (!await HelpCommand.PrintHelpText(Args.GroupId, Args.Param[0]))
                            {
                                ExecuteLogger.Error($"Unknown Command: <{Args.Param[0]}>, At Command <{Args.Command}>");
                                return;
                            }
                        }
                        else
                        {
                            _ = await HelpCommand.PrintHelpText(Args.GroupId, "help");
                        }
                    }
                    else if (Args.Command.Equals("symmet"))
                    {
                        //ParamFormat: [MsgId] [Pattern] or [Pattern] [ImageUrl]
                        if (Args.Param.Count > 1)
                        {
                            Image? Im = null;
                            MemoryStream ms = new();
                            string Pattern = "L";
                            if (Args.Param[1].StartsWith("http")){
                                //[Pattern] [ImageUrl]
                                byte[] ImageBytes = HttpService.GetBinary(Args.Param[1]);
                                ms = new MemoryStream(ImageBytes);
                                Im = Image.FromStream(ms);
                                Pattern = Args.Param[0];
                            }
                            else
                            {
                                //[MsgId] [Pattern]
                                JObject TargetMsg = await HttpApi.GetMsg(Args.Param[0]);
                                if (!(TargetMsg.Value<string>("message_id")?.Length > 0))
                                {
                                    ExecuteLogger.Error($"Invalid MsgId: {Args.Param[0]}, At Command <{Args.Command}>"); ;
                                    await HttpApi.SendPlainMsg(Args.GroupId, "无效的Msg");
                                }
                                else
                                {
                                    string PicUrl = CommandResolver.ExtractUrlFromMsg(TargetMsg);
                                    byte[] ImageBytes = HttpService.GetBinary(PicUrl);
                                    ms = new MemoryStream(ImageBytes);
                                    Im = Image.FromStream(ms);
                                    Pattern = Args.Param[1];
                                }
                            }
                            if (Im != null)
                            {
                                if (Im.RawFormat.Equals(ImageFormat.Gif))
                                {
                                    string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.gif");
                                    MagickImageCollection ResultImage = GifConvert.GifTransform(Im, Pattern);
                                    if (ResultImage.Count > 0)
                                    {
                                        ResultImage.Write(ImCachePath);
                                        await HttpApi.SendImageMsgWithReply(Args.GroupId,Args.MsgId, ImCachePath, ImageSendType.LocalFile);
                                        //await HttpApi.SendImageMsg(Args.GroupId, ImCachePath, ImageSendType.LocalFile);
                                        FileIO.SafeDeleteFile(ImCachePath);
                                    }
                                    else
                                    {
                                        ExecuteLogger.Error($"Pic Convert Failed, At Command <{Args.Command}>");
                                        await HttpApi.SendPlainMsg(Args.GroupId, "转换失败");
                                    }
                                    ResultImage.Dispose();
                                }
                                else
                                {
                                    string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                                    Bitmap ResultImage = PicConvert.PicTransform(new Bitmap(Im), Pattern);
                                    if (ResultImage != null)
                                    {
                                        ResultImage.Save(ImCachePath,ImageFormat.Gif);
                                        await HttpApi.SendImageMsgWithReply(Args.GroupId, Args.MsgId, ImCachePath, ImageSendType.LocalFile);
                                        //await HttpApi.SendImageMsg(Args.GroupId, ImCachePath, ImageSendType.LocalFile);
                                        FileIO.SafeDeleteFile(ImCachePath);
                                    }
                                    else
                                    {
                                        ExecuteLogger.Error($"Pic Convert Failed, At Command <{Args.Command}>");
                                        await HttpApi.SendPlainMsg(Args.GroupId, "转换失败");
                                    }
                                    ResultImage?.Dispose();
                                }
                                Im.Dispose();
                            }
                            ms.Close();
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("titleself"))
                    {
                        //ParamFormat: [Title]
                        if (Args.Param.Count > 0)
                        {
                            HttpApi.SetGroupSpecialTitle(Args.GroupId, Args.CallerUin, Args.Param[0]);
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("permission"))
                    {
                        //ParamFormat: Any
                        await HttpApi.SendPlainMsg(Args.GroupId, $"<{Args.CallerUin}>权限等级为 {Args.CallerPermissionLevel}");
                    }
                    else if (Args.Command.Equals("listop"))
                    {
                        //ParamFormat: Any
                        List<long> OPList = Program.GetOPManager().GetOPList(Args.GroupId);
                        string OutString = "";
                        foreach (long OPUin in OPList)
                        {
                            JObject Member = await HttpApi.GetGroupMember(Args.GroupId, OPUin);
                            if (Member.Value<string>("group_id")?.Length > 0)
                            {
                                OutString += $"{Member.Value<string>("nickname")} <{OPUin}>,";
                            }
                        }
                        await HttpApi.SendPlainMsg(Args.GroupId, OutString.Length > 0 ? OutString : "No OPs");
                    }
                    else if (Args.Command.Equals("ban"))
                    {
                        //Param Format: [TargetUin , Duration]
                        if (Args.Param.Count > 1)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel <= TargetPermissionLevel)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to {TargetPermissionLevel}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else if (!int.TryParse(Args.Param[1], out var duration))
                            {
                                ExecuteLogger.Error($"Invalid Duration: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, $"无效的时长: {Args.Param[1]}");
                            }
                            else
                            {
                                HttpApi.SetGroupBan(Args.GroupId, TargetUin, duration);
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("kick"))
                    {
                        //Param Format: [TargetUin]
                        if (Args.Param.Count > 0)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>"); ;
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel <= TargetPermissionLevel)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to {TargetPermissionLevel}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                HttpApi.GroupKick(Args.GroupId, TargetUin);
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("recall"))
                    {
                        //Param Format: [TargetMsgId]
                        if (Args.Param.Count > 0)
                        {
                            JObject TargetMsg = await HttpApi.GetMsg(Int32.TryParse(Args.Param[0], out int TargetMsgId) ? TargetMsgId : 0);
                            if (!(TargetMsg.Value<string>("message_id")?.Length > 0))
                            {
                                ExecuteLogger.Error($"Invalid MsgId: {Args.Param[0]}, At Command <{Args.Command}>"); ;
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的Msg");
                            }
                            else
                            {
                                long TargetUin = TargetMsg.Value<JObject>("sender")?.Value<int>("user_id") ?? 0;
                                TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                                if (TargetPermissionLevel == -1)
                                {
                                    ExecuteLogger.Error($"Invalid Target: {TargetUin}, At Command <{Args.Command}>"); ;
                                    await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                                }
                                else if (Args.CallerPermissionLevel <= TargetPermissionLevel)
                                {
                                    ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to {TargetPermissionLevel}, At Command <{Args.Command}>");
                                    await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                                }
                                else
                                {
                                    HttpApi.RecallGroupMsg(TargetMsgId);
                                    HttpApi.RecallGroupMsg(Args.MsgId);
                                }
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("settitle"))
                    {
                        //Param Format: [TargetUin , Title]
                        if (Args.Param.Count > 1)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel <= TargetPermissionLevel)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to {TargetPermissionLevel}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                HttpApi.SetGroupSpecialTitle(Args.GroupId, TargetUin, Args.Param[1]);
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("op"))
                    {
                        //Param Format: [TargetUin]
                        if (Args.Param.Count > 0)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 2)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 2, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else if (TargetPermissionLevel > 1)
                            {
                                ExecuteLogger.Warn($"Target Already Have Higher Permission Level: {TargetPermissionLevel}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "对方已有更高权限等级");
                            }
                            else
                            {
                                int ret = Program.GetOPManager().AddOP(Args.GroupId, TargetUin);
                                if (ret == 200)
                                {
                                    await HttpApi.SendPlainMsg(Args.GroupId, string.Format("已将<{0}>设为群管", TargetUin));
                                }
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("deop"))
                    {
                        //Param Format: [TargetUin]
                        if (Args.Param.Count > 0)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 2)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 2, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else if (TargetPermissionLevel > 1)
                            {
                                ExecuteLogger.Warn($"Target Already Have Higher Permission Level: {TargetPermissionLevel}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "对方已有更高权限等级");
                            }
                            else
                            {
                                int ret = Program.GetOPManager().RemoveOP(Args.GroupId, TargetUin);
                                if (ret == 200)
                                {
                                    await HttpApi.SendPlainMsg(Args.GroupId, $"已取消<{Args.Param[0]}>群管身份");
                                }
                            }
                        }
                        else
                        {
                            ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                        }
                    }
                    else if (Args.Command.Equals("admin"))
                    {
                        //Param Format: [TargetUin]
                        if (Args.Param.Count > 0)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 3)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 3, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                HttpApi.SetGroupAdmin(Args.GroupId, TargetUin, true);
                            }
                        }
                    }
                    else if (Args.Command.Equals("deadmin"))
                    {
                        //Param Format: [TargetUin]
                        if (Args.Param.Count > 0)
                        {
                            string TargetUin = Args.Param[0];
                            TargetPermissionLevel = await Program.GetPermissionManager().GetPermissionLevel(Args.GroupId, TargetUin);
                            if (TargetPermissionLevel == -1)
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 3)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 3, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                HttpApi.SetGroupAdmin(Args.GroupId, TargetUin, false);
                            }
                        }
                    }
                    else if (Args.Command.Equals("enable"))
                    {
                        //Param Format: [Command]
                        if (Args.Param.Count > 0)
                        {
                            if (!AvaliableCommands.Contains(Args.Param[0]))
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 3)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 3, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                Program.GetConfigManager().SetCommandStatus(Args.Param[0], true);
                            }
                        }
                    }
                    else if (Args.Command.Equals("disable"))
                    {
                        //Param Format: [Command]
                        if (Args.Param.Count > 0)
                        {
                            if (!AvaliableCommands.Contains(Args.Param[0]))
                            {
                                ExecuteLogger.Error($"Invalid Target: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "无效的目标");
                            }
                            else if (Args.CallerPermissionLevel < 3)
                            {
                                ExecuteLogger.Warn($"Not Enough Permission: {Args.CallerPermissionLevel} Compare to 3, At Command <{Args.Command}>");
                                await HttpApi.SendPlainMsg(Args.GroupId, "权限等级不足");
                            }
                            else
                            {
                                Program.GetConfigManager().SetCommandStatus(Args.Param[0], false);
                            }
                        }
                    }
                    ExecuteLogger.Info("Execute Finished");
                }
                else
                {
                    await HttpApi.SendPlainMsg(Args.GroupId, string.Format("已禁用指令: <{0}>", Args.Command));
                }
            }
            else
            {
                ExecuteLogger.Error($"Unknown Command: <{Args.Command}>");
                await HttpApi.SendPlainMsg(Args.GroupId, $"未知指令: <{Args.Command}>");
            }
        }
    }
}
