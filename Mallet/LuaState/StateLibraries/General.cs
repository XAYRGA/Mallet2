using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NLua;

namespace Mallet.ILuaState
{
    public static class General
    {
        private static Lua State;
        private static Stopwatch Systime_Value; 

        public static double SysTime()
        {
            return Systime_Value.Elapsed.TotalSeconds;
        }

        public static void writeError(string msg)
        {
            Mallet.LuaState.writeError(msg);
        }


        public static void InitLibrary(KLuaState state)
        {
            Console.WriteLine("State C Library: General");
            Systime_Value = Stopwatch.StartNew();

            var ILuaInterface = state.getLuaState();
            State = ILuaInterface;
        
            ILuaInterface.RegisterFunction("SysTime", null, typeof(General).GetMethod("SysTime"));
            ILuaInterface.RegisterFunction("writeError", null, typeof(General).GetMethod("writeError"));


        }
    }
}
