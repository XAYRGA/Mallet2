using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using NLua;

namespace Mallet.ILuaState
{
    public class HTTPRequestContainer
    {
        public string url { get; }
        public LuaFunction func { get; }

        public HTTPRequestContainer (string iurl,LuaFunction gah) {
            url = iurl;
            func = gah;
        }

    }

    public static class HTTP
    {
        private static Lua State;


        public static void Fetch(string url, LuaFunction fun)
        {
            Thread ReqThd = new Thread(new ParameterizedThreadStart(HandleHTTPRequest));
            ReqThd.Start(new HTTPRequestContainer(url, fun));
        }

        private static void HandleHTTPRequest(object HReq)
        {
            var Request = (HTTPRequestContainer)HReq;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Request.url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                string resp = "";


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    lock (State)
                    {
                        resp = reader.ReadToEnd();
                        Request.func.Call(true, resp, response.StatusCode); 
                    }
                }
              
            } catch (Exception E)
            {
                lock (State)
                {
                    try
                    {
                        Request.func.Call(false, E.ToString());
                    }
                    catch {
                        // cock
                        // nested try lock statement hnnngh. 
                        // fuck me right in the ass.

                    }


                }
            }

        }



        public static void InitLibrary(KLuaState state)
        {
            Console.WriteLine("State C Library: HTTP");
            state.runLua(" http = {}");

            var ILuaInterface = state.getLuaState();
            State = ILuaInterface;

            ILuaInterface.RegisterFunction("http.Fetch", null, typeof(HTTP).GetMethod("Fetch"));

         }
    }
}
