#nullable enable

namespace HH.Actors.Contracts
{
    using System;
    using UnityEngine;

    public delegate void CallbackEventHandler(string callbackID);

    public enum ActorCallbackState
    {
        None,
        Executing,
        Completed,
        Failed,
    }

    /// <summary>
    /// ActorCallBackSource is a subclass of ActorCallback. Its partial so that these two classes can be separated into two files.
    /// </summary>
    [Serializable]
    public partial class ActorCallback
    {
        public ActorCallback(string id, IDefaultActor actor)
        {
            InstantiatingActor = actor;
            ID = id;
        }

        public event CallbackEventHandler OnComplete = id => { };

        public event CallbackEventHandler OnFail = id => { };

        public IDefaultActor InstantiatingActor { get; private set; }

        public string ID { get; private set; } = string.Empty;

        public ActorCallbackState State { get; private set; } = ActorCallbackState.None;

        public void MarkExecuting()
        {
            State = ActorCallbackState.Executing;
        }

        public void MarkFailed()
        {
            if (State is not ActorCallbackState.Completed and not ActorCallbackState.Failed)
            {
                State = ActorCallbackState.Failed;
                OnFail?.Invoke(ID);
                return;
            }
        }

        public void TryComplete()
        {
            if (State is ActorCallbackState.Executing)
            {
                State = ActorCallbackState.Completed;
                OnComplete?.Invoke(ID);
            }
            else
            {
            }
        }
    }
}
