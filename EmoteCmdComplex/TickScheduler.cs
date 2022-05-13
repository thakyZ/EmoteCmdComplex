using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Dalamud.Game;
using Dalamud.Logging;

// borrowed from https://github.com/Eternita-S/NotificationMaster/blob/master/NotificationMaster/TickScheduler.cs
// ToDo: Deprecate when https://github.com/goatcorp/Dalamud/pull/832 is merged
namespace EmoteCmdComplex {
  internal class TickScheduler : IDisposable {
    internal static TickScheduler Schedule(Action function, Framework? framework = null, long delay = 0) {
      framework ??= Injections.Framework;

      return new TickScheduler(function, framework, delay);
    }

    internal static Task<T> RunOnNextFrame<T>(Func<T> function, Framework? framework = null, long delay = 0) {
      framework ??= Injections.Framework;

      var tcs = new TaskCompletionSource<T>();

#pragma warning disable S1481 // Unused local variables should be removed
      var _ = new TickScheduler(() => {
        try {
          tcs.SetResult(function.Invoke());
        } catch (Exception ex) {
          tcs.SetException(ex);
        }
      }, framework, delay);
#pragma warning restore S1481 // Unused local variables should be removed

      return tcs.Task;
    }

    private readonly long _executeAt;
    private readonly Action _function;
    private readonly Framework _framework;
    private bool _disposed;

    private TickScheduler(Action function, Framework framework, long delayMillis = 0) {
      this._executeAt = Environment.TickCount64 + delayMillis;
      this._function = function;
      this._framework = framework;
      framework.Update += this.Execute;
    }

    public void Dispose() {
      if (!this._disposed) {
        this._framework.Update -= this.Execute;
      }

      this._disposed = true;
    }

    private void Execute(object _) {
      if (Environment.TickCount64 < this._executeAt) return;

      try {
        this._function();
      } catch (Exception e) {
        PluginLog.Error(e, "Exception running a Framework tick event");
        Injections.Chat.PrintError($"[{EmoteCmdComplexPlugin.Instance.Name}] There was an issue running a task: {e.GetType()}: {e.Message}");
      }
      this.Dispose();
    }
  }
}
