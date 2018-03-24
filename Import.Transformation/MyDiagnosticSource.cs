using System;
using System.Diagnostics;

namespace Import.Transformation
{
    public class MyDiagnosticSource:DiagnosticSource
    {
        public override bool IsEnabled(string name)
        {
            return true;
        }

        public override void Write(string name, object value)
        {
            Console.WriteLine($"{name}:{value}");
        }
    }
}
