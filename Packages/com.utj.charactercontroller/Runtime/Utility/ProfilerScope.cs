using System;
using UnityEngine.Profiling;

namespace Unity.TinyCharacterController.Utility
{
    public readonly struct ProfilerScope : IDisposable
    {
        public ProfilerScope(string name)
        {
            Profiler.BeginSample(name);
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}
