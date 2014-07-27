using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class SomeObject
    {
        string name = "SomeName";
        public string Name { get { return name; } }
    }

    public enum SomeEnum
    {
        None,
        Some
    }

    public class SomeLogic
    {
        public int TheAnswer()
        {
            return 42;
        }

        public int AnotherAnswer
        {
            get { return 33; }
        }

        public SomeEnum Choose(bool input)
        {
            return input ? SomeEnum.Some : SomeEnum.None;
        }

        SomeObject some_object= new SomeObject();
        public SomeObject Object
        {
            get { return some_object; }
        }
    }
}
