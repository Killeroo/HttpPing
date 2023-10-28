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
        private bool _isExist = false;

        public bool IsExit => _isExist;

        public void Exit(int exitCode)
        {
            _isExist = true;
        }
    }
}
