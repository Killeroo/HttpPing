using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpPing.Interfaces
{
    internal interface IEnvironmentService
    {
        bool IsExit { get; }
        void Exit(int exitCode);
    }
}
