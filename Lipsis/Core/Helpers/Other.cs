using System;
using System.Diagnostics;

namespace Lipsis.Core {
    public static partial class Helpers {

        public static long Time(TimingCallback callback) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            callback();
            watch.Stop();
            return watch.ElapsedTicks;
        }
        public static long Time<T>(T state, TimingCallbackWithState<T> callback) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            callback(state);
            watch.Stop();
            return watch.ElapsedTicks;
        }

        public delegate void TimingCallback();
        public delegate void TimingCallbackWithState<T>(T state);
    }
}