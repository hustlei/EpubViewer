using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Contracts
{
    public interface IShell
    {
        void ProcessArgs(string[] args);
    }
}
