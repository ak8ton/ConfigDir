using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Model
{
    public interface ITypeError
    {
        int NotIntegerValue { get; }
        IOptions NotSubConfig { get; }
    }
}
