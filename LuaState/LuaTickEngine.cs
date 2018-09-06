using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NLua;


/* Generates the Tick hook for Lua state */


namespace Mallet.ILuaState
{
    public class LuaTickEngine
    {
        LuaFunction modhook_call;
        Timer TickInterval;
        long totalticks;
        Lua LState;
        public LuaTickEngine(Lua State,int fps)
        {
            modhook_call = State["ModHook.Call"] as LuaFunction;
            TickInterval = new Timer(1000 / fps);
            TickInterval.AutoReset = true;
            TickInterval.Elapsed += TickInterval_Elapsed;
            TickInterval.Enabled = true;
            LState = State;
        }
        private void TickInterval_Elapsed(object sender, ElapsedEventArgs e)
        {   try
            {
                lock (LState)
                {
                    totalticks++;
                    if (totalticks%300==0)
                    {
                        GC.Collect();
                        var mem = GC.GetTotalMemory(false);
                       // Console.WriteLine("Free {0} bytes.", mem);
                    }
                
                    modhook_call.Call("Tick");
                }
            } catch (Exception E)
            {
                var rawr = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Thaaaaat's not good. An error leaked back in the the C# state. Expect memory corruption. ");
                Console.WriteLine(E.ToString());
                Console.ForegroundColor = rawr;
                
            }
        }
    }
}
