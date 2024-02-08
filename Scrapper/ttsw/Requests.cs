using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace tts_wuxia
{
    static class Requests
    {
        private static HttpClient client = new HttpClient();
        public static string GetUrl(string url)
        {
        continue_error_to_many_request:
            try
            {
                string retour = string.Empty;
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0)");
                //var rand = new Random();
                //int delay = rand.Next(2_000, 10_000);
                //Thread.Sleep(delay);
                var rep = Task.Run(async () => await (client.GetStringAsync(url)));

                Task.WaitAll(new Task[] { rep });
                    retour = rep.Result;
                    return retour;

            }
            catch (System.Exception e)
            {

                if (e.Message.Contains("429"))
                {
                    Thread.Sleep(10_000);
                    goto continue_error_to_many_request;
                }

                throw;
            }
        }
    }
}   