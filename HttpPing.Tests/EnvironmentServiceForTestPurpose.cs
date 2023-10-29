using HttpPing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpPing.Tests
{
    internal class EnvironmentServiceForTestPurpose : IEnvironmentService
    {
        private bool _hasExited = false;

        public bool HasExited => _hasExited;

        public void Exit(int exitCode)
        {
            _hasExited = true;
        }
    }
}
