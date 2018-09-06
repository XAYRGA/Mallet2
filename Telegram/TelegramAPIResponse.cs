using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mallet.Telegram

{
    public struct TelegramAPIResponse
    {
       public bool success;
       public int code;
       public string data;
       public JToken tree;        

        
    }
}
