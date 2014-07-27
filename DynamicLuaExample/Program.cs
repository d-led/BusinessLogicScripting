using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLuaExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //using(
            dynamic lua = new DynamicLua.DynamicLua();
            //)
            {
                // hello world
                Console.WriteLine(lua("return 2+2"));

                // static configuration
                string user_static_config = @"
                    config = {
                        url = 'https://github.com/nrother/dynamiclua'
                    }
                ";

                lua(user_static_config);
                string url = (string)lua.config.url;
                Console.WriteLine(url);

                lua("config.answer = math.floor(131.94689145078/math.pi)");
                var answer = (double)lua.config.answer;
                Console.WriteLine(answer);
            }
        }
    }
}
