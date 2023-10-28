using HttpPing.Interfaces;
using System;
using System.Collections.Generic;

namespace HttpPing
{
    internal class EnvironmentService : IEnvironmentService
    {
        public bool IsExit { get; }

        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode); 
        }
    }
}
