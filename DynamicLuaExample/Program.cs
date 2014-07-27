using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLuaExample
{
    class Example
    {
        int answer = 42;
        public int Answer { get { return answer; } }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

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
                string url = lua.config.url;
                Console.WriteLine(url);

                // scripting the no more static configuration
                lua("config.answer = nil or math.floor(131.94689145078/math.pi)");
                int answer = (int)lua.config.answer;
                Console.WriteLine(answer);

                // as a function evaluator
                dynamic result = lua("return 1, 'world'");
                Console.WriteLine("{0} {1}", result[0], result[1]);

                // binding .NET classes and functions
                lua.NewExample = new Func<Example>(() => new Example());

                // Lua functions may be called via dynamiclua
                Example my_example = lua.NewExample();
                Console.WriteLine(my_example.Answer);

                lua(@"
                    local example = NewExample()
                    print(example.Answer)
                    print(example:Add(2,3))
                ");
            }
        }
    }
}
