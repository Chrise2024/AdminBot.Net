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
        private static string HttpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient HClient = new();

        public static void SendPlainMsg(string TargetGroupId,string MsgText)
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
        public static void SendImageMsg(string TargetGroupId, string ImageContent,string FileType = "LocalFile")
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
        public static void SetGroupBan(string TargetGroupId, string TargetUin,int Duration)
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
        public static void GroupKick(string TargetGroupId, string TargetUin,bool PermReject = false)
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
        public static void RecallGroupMsg(string MsgId,string TargetGroupId = "", string CurrentUin = "")
        {
            if (UInt32.TryParse(MsgId, out UInt32 IntMsgId))
            {
                try
                {
                    object ReqJSON = new
                    {
                        message_id = IntMsgId
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
        public static void SetGroupSpecialTitle(string TargetGroupId, string TargetUin, string Title)
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
        public static void SetGroupAdmin(string TargetGroupId, string TargetUin, bool Enable)
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
        public static JObject GetMsg(string MsgId)
        {
            if ( MsgId.Length == 0)
            {
                return JObject.Parse(@"{""message_id"":""""}");
            }
            if (UInt32.TryParse( MsgId, out UInt32 IntMsgId))
            {
                try
                {
                    object ReqJSON = new
                    {
                        message_id = IntMsgId
                    };
                    HttpResponseMessage response = HClient.PostAsync(HttpPostUrl + "/get_msg",
                       new StringContent(
                           JsonConvert.SerializeObject(ReqJSON),
                           Encoding.UTF8,
                           "application/json"
                       )
                    ).Result;
                    return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""message_id"":""""}");
                }
                catch
                {
                    return JObject.Parse(@"{""message_id"":""""}");
                }
            }
            else
            {
                return JObject.Parse(@"{""message_id"":""""}");
            }
            
        }
        public static JObject GetGroupMember(string TargetGroupId, string TargetUin)
        {
            if (TargetGroupId.Length == 0 || TargetUin.Length == 0)
            {
                return JObject.Parse(@"{""group_id"":""""}");
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
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<JObject>() ?? JObject.Parse(@"{""group_id"":""""}");
            }
            catch
            {
                return JObject.Parse(@"{""group_id"":""""}");
            }
        }
    }
}
