using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mallet.ILuaState;
using Mallet.Telegram;
namespace Mallet
{
    public static class Mallet
    {
       public static KroConfig Configuration;
       public static KLuaState LuaState;
       public static TelegramBot TBot;

       public static void Main(string[] args)
        {


            // Setup Configuration object//
            Configuration = new KroConfig("config.ini");



            if (Configuration.getValue("_completedfirstsetup", "false") == "false")
            {
                Configuration.getConfigInfo(); // sorry man.
            }

            TBot = new TelegramBot(Configuration.getValue("btoken", ""));

            LuaState = new KLuaState();

          


            enterCommandLoop();

            Console.ReadLine();

        }






        private static void enterCommandLoop()
        {


            while (true)
            {
                Console.Write(">");
                string command = Console.ReadLine();

                if (command == "lua")
                {
                    LuaState.luaLoop();
                }






            }


        }





    }
}
