using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using DomainLogic;

namespace BusinessLogicScripting
{
    [TestClass]
    public class DynamicLuaTest : IDisposable
    {
        dynamic lua = new DynamicLua.DynamicLua();

        void Dispose(bool should_dispose)
        {
            if (should_dispose)
                lua.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
            // excercising http://www.lua.org/bugs.html

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

        [TestMethod]
        public void ReturningEnums()
        {
            lua.logic = new SomeLogic();
            ((SomeEnum)lua("return logic:Choose(true)")[0]).Should().Be(SomeEnum.Some);
            ((SomeEnum)lua("return logic:Choose(false)")[0]).Should().Be(SomeEnum.None);
        }

        [TestMethod]
        public void ReturningOtherObjects()
        {
            lua.logic = new SomeLogic();
            var answer = lua("return logic.Object")[0] as SomeObject;
            answer.Should().NotBeNull();
            answer.Name.Should().Be(new SomeObject().Name);
        }

        [TestMethod]
        public void NoAccessToPrivates()
        {
            lua.obj = new SomeObject();

            new Action(() => lua("obj.name = 'blabla'"))
                .ShouldThrow<Exception>();

            string answer = lua("return obj.name")[0];
            answer.Should().NotBe(new SomeObject().Name);
        }

        [TestMethod]
        public void CreatingBoundObjects()
        {
            lua.NewObject = new Func<SomeObject>(() => new SomeObject());
            var answer = lua("return NewObject()")[0] as SomeObject;
            answer.Should().NotBeNull();
        }

        [TestMethod]
        public void AccessToOtherCLRBindingsOnlyOnDemand()
        {
            new Action(() => lua("print(System.Console.BufferHeight)"))
                .ShouldThrow<Exception>();

            lua.import("System");

            var func = new Func<int>(() => (int)lua("return Console.BufferHeight")[0]);
            new Action(() => func())
                .ShouldNotThrow();
            func().Should().Be(Console.BufferHeight);
        }

        [TestMethod]
        public void LoadingAssembliesAndFilesShouldBeOptional()
        {
            lua("import = nil");
            lua("require = nil");

            new Action(() => lua("import 'System'"))
                .ShouldThrow<Exception>();

            new Action(() => lua("require 'lfs'"))
                .ShouldThrow<Exception>();
        }
    }
}
