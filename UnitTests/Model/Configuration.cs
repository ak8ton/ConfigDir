using System;
using System.Collections.Generic;
using System.Text;
using ConfigDir;

namespace UnitTests.Model
{
    public abstract class Configuration : Config
    {
        public abstract IStend Stend { get; }
    }
}
