using System;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net;

namespace AdminBot.Net.NetWork
{
    public enum ImageSendType
    {
        LocalFile = 0,
        Url = 1,
        Base64 = 2
    }
    internal class HttpApi
    {
        private static readonly string HttpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient HClient = new();

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
            catch { }
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
            catch { }
        }
        public static async Task SendImageMsg<T>(T TargetGroupId, string ImageContent, ImageSendType ImageType = ImageSendType.LocalFile)
        {
            try
            {
                object ReqJSON;
                if (ImageType == ImageSendType.LocalFile)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = $"file:///{ImageContent}"
                                }
                            }
                        }
                    };
                }
                else if (ImageType == ImageSendType.Url)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = ImageContent
                                }
                            }
                        }
                    };
                }
                else if (ImageType == ImageSendType.Base64)
                {
                    ReqJSON = new
                    {
                        group_id = TargetGroupId,
                        message = new List<object>()
                        {
                            new {
                                type = "image",
                                data = new {
                                    file = $"base64://{ImageContent}"
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
            catch { }
        }
        public static async Task SendImageMsgWithReply<T1,T2>(T1 TargetGroupId, T2 ReplyMsgId, string ImageContent, ImageSendType ImageType = ImageSendType.LocalFile)
        {
            try
            {
                object ReqJSON;
                if (ImageType == ImageSendType.LocalFile)
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
                                    file = $"file:///{ImageContent}"
                                }
                            }
                        }
                    };
                }
                else if (ImageType == ImageSendType.Url)
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
                                    file = ImageContent
                                }
                            }
                        }
                    };
                }
                else if (ImageType == ImageSendType.Base64)
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
                                    file = $"base64://{ImageContent}"
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
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
            catch { }
        }
        public static async Task<JObject> GetMsg<T>(T MsgId)
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
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""message_id"":0}");
            }
            catch
            {
                return JObject.Parse(@"{""message_id"":0}");
            }
        }
        public static async Task<JObject> GetGroupMember<T1, T2>(T1 TargetGroupId, T2 TargetUin)
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
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""group_id"":0}");
            }
            catch
            {
                return JObject.Parse(@"{""group_id"":0}");
            }
        }
    }
}
