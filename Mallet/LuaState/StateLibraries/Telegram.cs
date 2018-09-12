using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NLua;
using Mallet.Telegram;
using System.Threading;


namespace Mallet.ILuaState
{
    public static class Telegram
    {
        private static Lua State;
        private static TelegramBot bot;
        private static long conf_index = 0;


        static LuaTable last_api_params;
        static LuaFunction last_api_callback;
        static string last_api_method;


        public static void getUpdates(LuaFunction callback)
        {
            Thread ReqThd = new Thread(new ParameterizedThreadStart(handleUpdates));
            ReqThd.Start(callback);
        }

        public static void apiPost(string method, LuaTable postparams ,LuaFunction callback)
        {
            last_api_callback = callback;
            last_api_params = postparams;
            last_api_method = method; 

            Thread ReqThd = new Thread(new ThreadStart(apiPost_Run));
            ReqThd.Start();
        }

        public static void apiPost_Run()
        {
            var callback = last_api_callback;
            var pparams = last_api_params;
            var method = last_api_method;
            lock (State)
            {
                var wtf = pparams.Keys.GetEnumerator();
                var wtf2 = pparams.Values.GetEnumerator();


                wtf.MoveNext();
                wtf2.MoveNext();

                var Params_Collection = new NameValueCollection();

                for (int i = 0; i < pparams.Keys.Count; i++)
                {

                    var k = wtf.Current.ToString();
                    var v = wtf2.Current.ToString();

                    Params_Collection[k] = v;
                    wtf.MoveNext();
                    wtf2.MoveNext();

                }

                var res = bot.apiGetRequest(method, Params_Collection);
                try
                {
                    callback.Call(res.data);

                } catch
                {

                }
             

            }
        }


        public static void confirmUpdate(int index)
        {
            conf_index = index;
        }

   
        private static void handleUpdates(object param)
        {
            var callback = (LuaFunction)param;
            
            lock (State)
            {
                var tbl = LuaUtil.EmptyTable(State);
                var container = LuaUtil.EmptyTable(State);
                try
                {
                    var asd = bot.getUpdates(conf_index);


                    container["data"] = asd.data;

                    

                    callback.Call(container);

                }
                catch (Exception e)
                {
                    tbl["error"] = true;
                    tbl["exception"] = e.ToString();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("telegram.GetUpdates: {0}", tbl["exception"]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
               
            }
            

        }



        public static void InitLibrary(KLuaState state)
        {
            Console.WriteLine("State C Library: Telegram");

            state.runLua(" telegram = {}");
            var ILuaInterface = state.getLuaState();
            State = ILuaInterface;

            bot = Mallet.TBot;

            ILuaInterface.RegisterFunction("telegram.getUpdates", null, typeof(Telegram).GetMethod("getUpdates"));
            ILuaInterface.RegisterFunction("telegram.confirmUpdate", null, typeof(Telegram).GetMethod("confirmUpdate"));
            ILuaInterface.RegisterFunction("telegram.apiPostRaw", null, typeof(Telegram).GetMethod("apiPost"));

        }
    }
}
