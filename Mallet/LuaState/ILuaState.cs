using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;

namespace Mallet.ILuaState
{
    public class KLuaState
    {
        private Lua IState;

        private LuaTickEngine TickGen;


        public KLuaState()
        {
            IState = new Lua();


            runLua(@"
                import = function () end


                print(""Starting preloader...."")
       

                dofile('Mallet/preinit.lua')

            ");

            File.InitLibrary(this);
            General.InitLibrary(this);
            HTTP.InitLibrary(this);
            Telegram.InitLibrary(this);
            sql.InitLibrary(this);

            reloadLua();


            int fps = Convert.ToInt32(Mallet.Configuration.getValue("MaxFPS", "60"));
            TickGen = new LuaTickEngine(IState, fps);
           


        }

        public void reloadLua()
        {
            runLua(@"
                import = function () end


                print(""Mallet Lua state loaded. Running Lua 5.2.2"")
       

                dofile('Mallet/init.lua')

            ");
        }

        public Lua getLuaState()
        {
            return IState;
        }
        public void writeError(string err)
        {
            var what = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(err);
            Console.ForegroundColor = what;
        }


        public string runLua(string script)
        {
            string exception = null;
            try
            {
                lock (IState)
                {
                    IState.DoString(script);
                }
            } catch (Exception Error)
            {
                exception = Error.Message;
                writeError(exception);
            }
            return exception;
        }

        public void luaLoop()
        {
            Console.WriteLine();
            var what = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dropped to Lua interpreter.");
            Console.WriteLine("Type !! at any time to restore Mallet prompt.");
            Console.ForegroundColor = what;

            while (true)
            {
                Console.Write(">>");
                string command = Console.ReadLine();
                if (command == "!!") {
                      var what2 = Console.ForegroundColor;
                      Console.ForegroundColor = ConsoleColor.Green;
                      Console.WriteLine("Returning to Mallet prompt.");
                      Console.ForegroundColor = what2;
                    break;
                }
                runLua(command);
            }


        }
    }
}
