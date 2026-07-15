#nullable enable
namespace HH.Input.Networking
{
    using System;
    using UnityEngine;

    [Serializable]
    public class NoNetworkInput : INetworkedActionInput
    {
        public void Start(NetworkedInputState input)
        {
            Debug.LogWarning($"{input.ActorID} tried activating an input with no logic");
        }

        public void Update(NetworkedInputState input)
        {
            Debug.LogWarning($"{input.ActorID} tried activating an input with no logic");
        }

        public void Cancel(NetworkedInputState input)
        {
            Debug.LogWarning($"{input.ActorID} tried activating an input with no logic");
        }

        public void Failed(NetworkedInputState input)
        {
            Debug.LogWarning($"{input.ActorID} tried activating an input with no logic");
        }
    }
}
