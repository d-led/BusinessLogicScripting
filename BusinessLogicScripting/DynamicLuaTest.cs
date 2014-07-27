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
            ((int)lua.logic.TheAnswer()).Should().Be(logic.TheAnswer());

            ((int)lua.logic.AnotherAnswer).Should().Be(logic.AnotherAnswer);
        }
    }
}
