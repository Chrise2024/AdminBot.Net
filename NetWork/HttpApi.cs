using System;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net;
using AdminBot.Net.Utils;

namespace AdminBot.Net.NetWork
{
    public enum ImageSendType
    {
        LocalFile = 0,
        Url = 1,
        Base64 = 2
    }
    public enum ImageSubType
    {
        Normal = 0,  //正常图片
        Emoji = 1,   //表情包
        Hot = 2,     //热图
        Battel = 3,  //斗图
    }
    internal class HttpApi
    {
        private static readonly string HttpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient HClient = new();

        private static readonly Logger HApi = new("HttpService");

        public static async Task SendPlainMsg<T>(T TargetGroupId,string MsgText)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    message = new List<object>()
                {
                    new {
                        type = "text",
                        data = new {
                            text = MsgText
                        }
                    }
                }
                };
                await HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task SendReplyMsg<T1,T2>(T1 TargetGroupId,T2 ReplyMsgId, string MsgText)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    message = new List<object>()
                {
                    new {
                        type = "reply",
                        data = new {
                            id = $"{ReplyMsgId}"
                        }
                    },
                    new {
                        type = "text",
                        data = new {
                            text = MsgText
                        }
                    }
                }
                };
                await HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task SendImageMsg<T>(T TargetGroupId, string ImageContent, ImageSendType SendType = ImageSendType.LocalFile,ImageSubType SubType = ImageSubType.Normal)
        {
            try
            {
                object ReqJSON;
                if (SendType == ImageSendType.LocalFile)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = $"file:///{ImageContent}",
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else if (SendType == ImageSendType.Url)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = ImageContent,
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else if (SendType == ImageSendType.Base64)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = $"base64://{ImageContent}",
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else
                {
                    return;
                }
                await HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task SendImageMsgWithReply<T1,T2>(T1 TargetGroupId, T2 ReplyMsgId, string ImageContent, ImageSendType SendType = ImageSendType.LocalFile, ImageSubType SubType = ImageSubType.Normal)
        {
            try
            {
                object ReqJSON;
                if (SendType == ImageSendType.LocalFile)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "reply",
                                data = new {
                                    id = $"{ReplyMsgId}"
                                }
                            },
                            new {
                                type = "image",
                                data = new {
                                    file = $"file:///{ImageContent}",
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else if (SendType == ImageSendType.Url)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "reply",
                                data = new {
                                    id = $"{ReplyMsgId}"
                                }
                            },
                            new {
                                type = "image",
                                data = new {
                                    file = ImageContent,
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else if (SendType == ImageSendType.Base64)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "reply",
                                data = new {
                                    id = $"{ReplyMsgId}"
                                }
                            },
                            new {
                                type = "image",
                                data = new {
                                    file = $"base64://{ImageContent}",
                                    subType = SubType
                                }
                            }
                        }
                    };
                }
                else
                {
                    return;
                }
                await HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async void SetGroupBan<T1, T2>(T1 TargetGroupId, T2 TargetUin,int Duration)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    duration = Duration
                };
                await HClient.PostAsync(HttpPostUrl + "/set_group_ban",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async void GroupKick<T1, T2>(T1 TargetGroupId, T2 TargetUin, bool PermReject = false)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    reject_add_request = PermReject
                };
                await HClient.PostAsync(HttpPostUrl + "/set_group_kick",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async void RecallGroupMsg<T>(T MsgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = MsgId
                };
                await HClient.PostAsync(HttpPostUrl + "/delete_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async void SetGroupSpecialTitle<T1, T2>(T1 TargetGroupId, T2 TargetUin, string Title)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    special_title = Title
                };
                await HClient.PostAsync(HttpPostUrl + "/set_group_special_title",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async void SetGroupAdmin<T1,T2>(T1 TargetGroupId, T2 TargetUin, bool Enable)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    enable = Enable
                };
                await HClient.PostAsync(HttpPostUrl + "/set_group_admin",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (Exception ex)
            {
                HApi.Error("Error Occured, Error Information:");
                HApi.Error(ex.Message);
                HApi.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task<MsgBodySchematics> GetMsg<T>(T MsgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = MsgId
                };
                HttpResponseMessage response = await HClient.PostAsync(HttpPostUrl + "/get_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<MsgBodySchematics>() ?? new MsgBodySchematics();
            }
            catch
            {
                return new MsgBodySchematics();
            }
        }
        public static async Task<GroupMemberSchematics> GetGroupMember<T1, T2>(T1 TargetGroupId, T2 TargetUin)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    no_cache = false
                };
                HttpResponseMessage response = await HClient.PostAsync(HttpPostUrl + "/get_group_member_info",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<GroupMemberSchematics>() ?? new GroupMemberSchematics();
            }
            catch
            {
                return new GroupMemberSchematics();
            }
        }
        public static async Task<HitokotoSchematics> GetHitokoto(string HType)
        {
            string Para = "";
            foreach(char index in HType)
            {
                if (index >= 'a' && index <= 'l')
                {
                    Para += $"c={index}&";
                }
            }
            try
            {
                HttpResponseMessage response = await HClient.GetAsync("https://v1.hitokoto.cn/?" + Para);
                return JsonConvert.DeserializeObject<HitokotoSchematics>(response.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                return new HitokotoSchematics();
            }
        }
        public static async Task<Image> GetQQAvatar<T>(T TargetUin)
        {
            try
            {
                byte[] ImageBytes = await HClient.GetByteArrayAsync($"http://q.qlogo.cn/headimg_dl?dst_uin={TargetUin}&spec=640&img_type=jpg");
                if (ImageBytes.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(ImageBytes);
                    Image Im = Image.FromStream(ms);
                    ms.Close();
                    return Im;
                }
                else
                {
                    return Image.FromFile(Path.Join(Program.GetProgramRoot(), "Local", "512x512.png"));
                }
            }
            catch
            {
                return Image.FromFile(Path.Join(Program.GetProgramRoot(),"Local", "512x512.png"));
            }
        }
    }
}
