using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using DomainLogic;

namespace BusinessLogicScripting
{
    [TestClass]
    public class DynamicLuaTest
    {
        dynamic lua = new DynamicLua.DynamicLua();

        [TestMethod]
        public void SimpleScripting()
        {
            lua("result = { a = 42, b = 33 }");

            ((int)lua["result.a"]).Should().Be(42);
            ((int)lua.result.a).Should().Be(42);
        }

        [TestMethod]
        public void BoundFunctions()
        {
            SomeLogic logic = new SomeLogic();
            lua.TheAnswer = new Func<int>(() => logic.TheAnswer());

            ((int)lua.TheAnswer()[0]).Should().Be(logic.TheAnswer());

            lua.logic = logic;
            dynamic answer = lua("return logic:TheAnswer()");
            ((int)answer[0]).Should().Be(logic.TheAnswer());

            ((int)lua.logic.AnotherAnswer).Should().Be(logic.AnotherAnswer);
            ((int)lua("return logic.AnotherAnswer")[0]).Should().Be(logic.AnotherAnswer);
        }

        [TestMethod]
        public void ExceptionShouldBeThrownOnSyntaxError()
        {
            Action script = () => lua("a[)");
            script.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void ExceptionShouldBeThrownOnRuntimeError()
        {
            Action script = () => lua("a()");
            script.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void KnownCrashingBugsShouldBeControllable()
        {
            //http://www.lua.org/bugs.html

            Action[] actions = new Action[] {
                ()=>lua(@"table.unpack({}, 0, 2^31 - 1)"),
                ()=>lua(@"function f() f() end f()"),
                ()=>lua(@"print(string.find(string.rep('a', 2^20), string.rep('.?', 2^20)))"),
                ()=>lua(@"print(unpack({1,2,3}, 2^31-1, 2^31-1))"),
                ()=>lua(@"a = string.dump(function()return;end)
                            a = a:gsub(string.char(30,37,122,128), string.char(34,0,0), 1)
                            loadstring(a)()")
            };

            foreach (var action in actions)
            {
                action.ShouldThrow<Exception>();
            }
        }
    }
}
