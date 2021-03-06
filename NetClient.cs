using Newtonsoft.Json;

namespace UptoboxYui
{
    public class Api
    {
        static readonly HttpClient client = new HttpClient();
        static string HomeApi = "https://uptobox.com/";
        /// <summary>
        /// 获取链接数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> HttpGet(string url)
        {
            string responseBody = "";
            int forint = 0;
        HttpGet:
            try
            {
                //实例化http服务传入链接
                HttpResponseMessage response = await client.GetAsync(url);

                //获取http状态码
                //Console.WriteLine(Convert.ToInt32(response.StatusCode));

                //成功后清除输出的等待文本动画
                if (response.IsSuccessStatusCode)
                {
                    if (forint > 1)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        Console.Write("                 ");
                        Console.SetCursorPosition(0, Console.CursorTop);
                    }
                }

                //不成功引发异常
                response.EnsureSuccessStatusCode();

                //获取链接内容
                responseBody = await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException e)
            {

                //输出异常
                Console.WriteLine("连接失败, 等待10秒后重式, 重试次数 :{0}, 详细消息 :{1}", forint, e.Message);
                Console.CursorVisible = false;
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("少女祈祷中   ");
                    Task.Delay(1000).Wait();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("少女祈祷中.  ");
                    Task.Delay(1000).Wait();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("少女祈祷中.. ");
                    Task.Delay(1000).Wait();
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("少女祈祷中...");
                    Task.Delay(1000).Wait();
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.CursorVisible = true;
                forint++;
                goto HttpGet;
            }
            return responseBody;
        }
        /// <summary>
        /// 获取文件属性
        /// </summary>
        /// <param name="FileCode"></param>
        /// <returns></returns>
        public static async Task<Json.FilesInfo> FilesInfo(string fileCode)
        {
            string mes = await HttpGet(HomeApi + "api/link/info?fileCodes=" + fileCode);
            Console.WriteLine(mes);
            Json.FilesInfo json = JsonConvert.DeserializeObject<Json.FilesInfo>(mes);
            Console.WriteLine(Json.StatusCode(json.statusCode)+"1");
            return json;
        }
        public static async Task<Json.GetWaitingToken> GetWaitingToken(string userToken,string fileCode)
        {
            string mes = await HttpGet(HomeApi + "api/link?token=" + userToken + "&file_code=" + fileCode);
            Console.WriteLine(mes);
            Json.GetWaitingToken json = JsonConvert.DeserializeObject<Json.GetWaitingToken>(mes);
            Console.WriteLine(Json.StatusCode(json.statusCode)+"2");
            return json;
        }
        public static async Task<Json.GetDownloadLink> GetDownloadLink(string userToken,string fileCode,string waitingToken)
        {
            string mes = await HttpGet(HomeApi + "api/link?token=" + userToken + "&file_code=" + fileCode + "&waitingToken=" + waitingToken);
            Console.WriteLine(mes);
            Json.GetDownloadLink json = JsonConvert.DeserializeObject<Json.GetDownloadLink>(mes);
            Console.WriteLine(Json.StatusCode(json.statusCode)+"3");
            return json;
        }
    }
    public class Json
    {
        public static string StatusCode(int Code)
        {
            string[] status = new string[]
            {
                "0成功",
                "1发生错误",
                "2凭据无效",
                "3至少需要一个值",
                "4格式错误",
                "5需要高级帐户",
                "6缺少参数",
                "7参数无效",
                "8信用不足",
                "9不支持的字段",
                "10不可处理的请求",
                "11失败尝试次数过多",
                "12缺少令牌",
                "13令牌无效",
                "14优惠券已使用",
                "15不支持的转换",
                "16需要等待",
                "17需要密码",
                "18数据未更改",
                "19无效凭证",
                "20不可用",
                "21权限被拒绝",
                "22授予的权限",
                "23请求失败",
                "24请求部分成功",
                "25服务器暂时不可用",
                "26密码无效",
                "27需要经过身份验证的用户",
                "28未找到文件",
                "29您已达到今天的流式传输限制",
                "30未找到文件夹",
                "31需要一个数值",
                "32已达到限制",
                "33未知错误",
                "34无效数据",
                "35文件将很快可用",
                "36未找到订阅",
                "37流不可用",
                "38文件大小需要高级版",
                "39在请求新的下载链接之前，您需要等待"
            };
            return status[Code];
        }
        public class FilesInfo
        {
            public int statusCode { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
            public class Data
            {
                public List<ListItem> list { get; set; }
            }
            public class ListItem
            {
                public string file_code { get; set; }
                public string file_name { get; set; }
                public string file_size { get; set; }
                public string available_uts { get; set; }
                public string need_premium { get; set; }
                public string subtitles { get; set; }
            }
        }
        public class GetWaitingToken
        {
            public int statusCode { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
            public class Data
            {
                public int waiting { get; set; }
                public string waitingToken { get; set; }
            }
        }
        public class GetDownloadLink
        {
            public int statusCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string message { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Data data { get; set; }
            public class Data
            {
                /// <summary>
                /// 
                /// </summary>
                public string dlLink { get; set; }
            }
        }
    }

}
