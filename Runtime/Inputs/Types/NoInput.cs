#nullable enable
namespace HH.Input
{
    using System;
    using HH.Input.Contracts;
    using UnityEngine;

    [Serializable]
    public class NoInput : IActionInput
    {
        public void Start(InputCallback inputCallback)
        {
            Debug.LogWarning($"{inputCallback.Actor} tried activating an input with no logic");
        }

        public void Update(InputCallback inputCallback)
        {
            Debug.LogWarning($"{inputCallback.Actor} tried activating an input with no logic");
        }

        public void Cancel(InputCallback inputCallback)
        {
            Debug.LogWarning($"{inputCallback.Actor} tried activating an input with no logic");
        }

        public void Failed(InputCallback inputCallback)
        {
            Debug.LogWarning($"{inputCallback.Actor} tried activating an input with no logic");
        }
    }
}
