using System;
using System.Collections;
using System.Threading;

public abstract class BaseCoroutine
{
    protected bool Running { get; private set; }

    protected abstract IEnumerator Run(string message, Action<string> action);

    public virtual IEnumerator Start(string message, Action<string> action)
    {
        Running = true;
        yield return Run(message, action);
    }

    public virtual IEnumerator Stop()
    {
        Running = false;
        yield return null;
    }
}