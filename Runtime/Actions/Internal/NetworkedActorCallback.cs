#nullable enable

namespace HH.Actors.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ActorCallBackSource is a subclass of ActorCallback. Its partial so that these two classes can be separated into two files.
    /// </summary>
    [Serializable]
    public partial class NetworkedActorCallback
    {
        public NetworkedActorCallback(string id, IDefaultActor actor)
        {
            InstantiatingActor = actor;
            ID = id;
        }

        public event CallbackEventHandler OnComplete = id => { };

        public event CallbackEventHandler OnFail = id => { };

        public static Dictionary<string, NetworkedActorCallback> KnownCallbacks { get; private set; } = new();

        public IDefaultActor InstantiatingActor { get; private set; }

        public string ID { get; private set; } = string.Empty;

        public ActorCallbackState State { get; private set; } = ActorCallbackState.None;

        public static NetworkedActorCallback GetCallback(string id)
        {
            return KnownCallbacks[id];
        }

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
