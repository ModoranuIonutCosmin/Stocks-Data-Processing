using System;

namespace Stocks_Data_Processing.Quartz_Helpers
{

    public interface IScopedDependency
    {
        string Scope { get; }
    }

    class ScopedDependency : IScopedDependency
    {
        public ScopedDependency(string scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public string Scope { get; }
    }
}

