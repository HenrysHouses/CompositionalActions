#nullable enable

namespace HH.Actors.Contracts
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// ActorCallBackSource is a subclass of ActorCallback. Its partial so that these two classes can be separated into two files.
    /// </summary>
    public partial class ActorCallback
    {
        public class ActorCallbackSource
        {
            private static int sourcesCreated;
            private readonly int sourceID;
            private readonly IDefaultActor owner = new NoActor();
            private readonly Dictionary<string, ActorCallback> callbacks = new Dictionary<string, ActorCallback>();
            private int callbacksCreated = 0;

            public ActorCallbackSource(IDefaultActor owner)
            {
                sourceID = sourcesCreated++;
                this.owner = owner;
            }

            public bool OwnsCallback(string callbackID)
            {
                return callbacks.ContainsKey(callbackID);
            }

            public void MarkCallbackAsFailed(string callbackID)
            {
                callbacks[callbackID].MarkFailed();
            }

            // private void MarkCallbackCompleted(string ID)
            // {
            //     callbacks[ID].OnComplete.Invoke(ID);
            // }

            public ActorCallback? GetCompletedCallback(string callbackID)
            {
                ActorCallback completedCallback = callbacks[callbackID];

                if (completedCallback.State is not ActorCallbackState.Completed)
                {
                    return null;
                }

                return completedCallback;
            }

            public ActorCallback CreateCallback()
            {
                callbacksCreated++;
                string creationID = $"T:{Time.time}-CallB:{callbacksCreated}-Source:{sourceID}";

                ActorCallback newCallback = new ActorCallback(creationID, owner);
                callbacks.Add(creationID, newCallback);

                return newCallback;
            }
        }
    }
}
