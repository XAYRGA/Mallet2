using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net;

namespace Mallet.Telegram
{
    public partial class TelegramBot
    {
        string apitoken;
        string apipath = "";
        public TelegramBot(string apikey)
        {
            apitoken = apikey;
            apipath = "https://api.telegram.org/" + apikey + "/";
        }



       public TelegramAPIResponse apiGetRequest(string req, NameValueCollection para)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] response =
                    client.UploadValues(apipath + req, para);




                    string result = System.Text.Encoding.UTF8.GetString(response);

                    JObject newtree = JObject.Parse(result);

                    return new TelegramAPIResponse
                    {
                        success = (bool)newtree["ok"],
                        tree = newtree["result"],
                        data = result,
                        code = 0,

                    };
                } catch (WebException F)
                {
                    return new TelegramAPIResponse
                    {
                        success = false,

                        data = F.ToString(),
                        code = 0,

                    };
                }

            }



        }


        public TelegramAPIResponse getUpdates(long offset = 0)
        {
            return apiGetRequest("getUpdates", new NameValueCollection() { { "offset", offset.ToString() } });
        }
    }
}
