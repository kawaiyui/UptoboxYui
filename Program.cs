using CommandLine;
using CommandLine.Text;
using System.Timers;

namespace UptoboxYui
{
    class Program
    {
        private class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "HelpTextVerbose", ResourceType = typeof(Resource))]
            public bool verbose { get; set; }

            [Option('d', "debug", Required = false, HelpText = "HelpTextDebug", ResourceType = typeof(Resource))]
            public bool Debug { get; set; }

            [Option('o', "output-directory", Required = false, HelpText = "HelpTextOutput", ResourceType = typeof(Resource))]
            public string OutputDirectory { get; set; }

            [Option('t', "token", Required = true, HelpText = "HelpTextToken", ResourceType = typeof(Resource))]
            public string UserToken { get; set; }

            [Value(0, Required = true, HelpText = "HelpTextLinks", ResourceType = typeof(Resource))]
            public IReadOnlyList<string> Links { get; set; }

        }

        public static async Task Main(string[] args)
        {
            SentenceBuilder.Factory = () => new LocalizableSentenceBuilder();
            Options opts = default;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(parsed => opts = parsed)
                .WithNotParsed(_ => Environment.Exit(1));
            if (opts.verbose)
            {
                Console.WriteLine(opts.verbose);
            }
            //Console.WriteLine(await Api.HttpGet(@"https://uptobox.com/"));
            //Console.WriteLine(await Api.FilesInfo("of8ctaj7sact"));
            //Json.FilesInfo a = await Api.FilesInfo("of8ctaj7sact");
            //Console.WriteLine(a.data.list[0].file_name);
            //Console.WriteLine("");
            foreach(string i in opts.Links)
            {
                Start start = new Start();
                await start.StartTask(opts.UserToken, i);
            }
            
        }
        private class Start
        {


            private static System.Timers.Timer aTimer;
            private static bool Wait { get; set; }
            public async Task StartTask(string userToken, string fileCode)
            {
                Console.WriteLine("文件代码：" + fileCode);
                Json.FilesInfo a = await Api.FilesInfo(fileCode);
                Console.WriteLine("文件名：");
                Console.Write(a.data.list[0].file_name);
                Json.GetWaitingToken b = await Api.GetWaitingToken(userToken, fileCode);
                Console.WriteLine("等待时间：" + b.data.waiting + "等待代码" + b.data.waitingToken);
                if (b.data.waitingToken != null)
                {
                    await WaitTask(b.data.waiting);
                }
                Json.GetDownloadLink c = await Api.GetDownloadLink(userToken, fileCode, b.data.waitingToken);
                Console.WriteLine(c.data.dlLink);

            }
            public static async Task WaitTask(int time)
            {
                aTimer = new System.Timers.Timer(time * 1000);
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = false;
                //aTimer.Enabled = true;
                Wait = true;
                aTimer.Start();
                while (Wait == true)
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
            }
            private static void OnTimedEvent(Object source, ElapsedEventArgs e)
            {
                Wait = false;
                Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss}",
                      e.SignalTime);
            }
        }
    }
}