using System;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AdminBot.Net;

namespace AdminBot.Net.NetWork
{
    internal class HttpApi
    {
        private static readonly string HttpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient HClient = new();

        public static void SendPlainMsg(long TargetGroupId,string MsgText)
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
                HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void SendImageMsg(long TargetGroupId, string ImageContent,string FileType = "LocalFile")
        {
            try
            {
                object ReqJSON;
                if (FileType.Equals("LocalFile"))
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
                else if (FileType.Equals("Url"))
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
                else if (FileType.Equals("Base64"))
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
                var _ = HClient.PostAsync(HttpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                ).Result;
            }
            catch { }
        }
        public static void SetGroupBan(long TargetGroupId, long TargetUin,int Duration)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    duration = Duration
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_ban",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void SetGroupBan(long TargetGroupId, string TargetUin, int Duration)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    duration = Duration
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_ban",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void GroupKick(long TargetGroupId, long TargetUin,bool PermReject = false)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    reject_add_request = PermReject
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_kick",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void GroupKick(long TargetGroupId, string TargetUin, bool PermReject = false)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    reject_add_request = PermReject
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_kick",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void RecallGroupMsg(int MsgId,long TargetGroupId = 0, int CurrentUin = 0)
        {
            if (MsgId != 0)
            {
                try
                {
                    object ReqJSON = new
                    {
                        message_id = MsgId
                    };
                    HClient.PostAsync(HttpPostUrl + "/delete_msg",
                       new StringContent(
                           JsonConvert.SerializeObject(ReqJSON),
                           Encoding.UTF8,
                           "application/json"
                       )
                    );
                }
                catch { }
            }
        }
        public static void SetGroupSpecialTitle(long TargetGroupId, long TargetUin, string Title)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    special_title = Title
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_special_title",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void SetGroupSpecialTitle(long TargetGroupId, string TargetUin, string Title)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    special_title = Title
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_special_title",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void SetGroupAdmin(long TargetGroupId, long TargetUin, bool Enable)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    enable = Enable
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_admin",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static void SetGroupAdmin(long TargetGroupId, string TargetUin, bool Enable)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    enable = Enable
                };
                HClient.PostAsync(HttpPostUrl + "/set_group_admin",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch { }
        }
        public static JObject GetMsg(int MsgId)
        {
            if ( MsgId == 0)
            {
                return JObject.Parse(@"{""message_id"":0}");
            }
            else
            {
                try
                {
                    object ReqJSON = new
                    {
                        message_id = MsgId
                    };
                    HttpResponseMessage response = HClient.PostAsync(HttpPostUrl + "/get_msg",
                       new StringContent(
                           JsonConvert.SerializeObject(ReqJSON),
                           Encoding.UTF8,
                           "application/json"
                       )
                    ).Result;
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""message_id"":0}");
                }
                catch
                {
                    return JObject.Parse(@"{""message_id"":0}");
                }
            }
        }
        public static JObject GetMsg(string MsgId)
        {
            if (MsgId.Length == 0)
            {
                return JObject.Parse(@"{""message_id"":0}");
            }
            else
            {
                try
                {
                    object ReqJSON = new
                    {
                        message_id = MsgId
                    };
                    HttpResponseMessage response = HClient.PostAsync(HttpPostUrl + "/get_msg",
                       new StringContent(
                           JsonConvert.SerializeObject(ReqJSON),
                           Encoding.UTF8,
                           "application/json"
                       )
                    ).Result;
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""message_id"":0}");
                }
                catch
                {
                    return JObject.Parse(@"{""message_id"":0}");
                }
            }
        }
        public static JObject GetGroupMember(long TargetGroupId, long TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin == 0)
            {
                return JObject.Parse(@"{""group_id"":0}");
            }
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    no_cache = false
                };
                HttpResponseMessage response = HClient.PostAsync(HttpPostUrl + "/get_group_member_info",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                ).Result;
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""group_id"":0}");
            }
            catch
            {
                return JObject.Parse(@"{""group_id"":0}");
            }
        }
        public static JObject GetGroupMember(long TargetGroupId, string TargetUin)
        {
            if (TargetGroupId == 0 || TargetUin.Length == 0)
            {
                return JObject.Parse(@"{""group_id"":0}");
            }
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    no_cache = false
                };
                HttpResponseMessage response = HClient.PostAsync(HttpPostUrl + "/get_group_member_info",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                ).Result;
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""group_id"":0}");
            }
            catch
            {
                return JObject.Parse(@"{""group_id"":0}");
            }
        }
    }
}
