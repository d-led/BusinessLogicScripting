using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace BusinessLogicScripting
{
    [TestClass]
    public class DynamicLuaTest
    {
        [TestMethod]
        public void SimpleScripting()
        {
            dynamic lua = new DynamicLua.DynamicLua();

            lua("result = { a = 42, b = 33 }");

            ((int)lua["result.a"]).Should().Be(42);
            ((int)lua.result.a).Should().Be(42);
        }
    }
}
