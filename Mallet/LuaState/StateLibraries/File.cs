using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLua;

namespace Mallet.ILuaState
{
   public static class File
    {
        private static Lua State;

        public static string Read(string path)
        {
            
            string ret = null;
            try
            {
                ret = System.IO.File.ReadAllText(path);
            } catch
            {

            }
            return ret;
        }

        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static bool DirectoryExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public static void Write(string path,string content)
        {
            System.IO.File.WriteAllText(path,content);
        }

        public static LuaTable Find(string path,string pattern)
        {   try
            {
                return LuaUtil.stringArrayToTable(System.IO.Directory.GetFiles(path, pattern), State);
            } catch
            {
                return LuaUtil.EmptyTable(State);
            }
        }


        public static void InitLibrary(KLuaState state)
        {
            Console.WriteLine("State C Library: File");
            var ILuaInterface = state.getLuaState();
            State = ILuaInterface;
            state.runLua(" file = {}");
            ILuaInterface.RegisterFunction("file.Read", null,typeof(File).GetMethod("Read"));
            ILuaInterface.RegisterFunction("file.Exists", null, typeof(File).GetMethod("Exists"));
            ILuaInterface.RegisterFunction("file.Write", null, typeof(File).GetMethod("Write"));
            ILuaInterface.RegisterFunction("file.DirectoryExists", null, typeof(File).GetMethod("DirectoryExists"));
            ILuaInterface.RegisterFunction("file.Find", null, typeof(File).GetMethod("Find"));

        }
    }
}
