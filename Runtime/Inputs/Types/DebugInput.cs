#nullable enable
namespace HH.Input
{
    using System;
    using HH.Input.Contracts;
    using UnityEngine;

    [Serializable]
    public class DebugInput : IActionInput
    {
        [SerializeField] private int testInt = 0;
        [SerializeField] private bool testBool = true;

        public void Start(InputCallback callbackContext)
        {
            if (!testBool)
            {
                callbackContext.MarkAsFailed();
                Debug.LogError("Failed to start action, testBool is false.");
                return;
            }

            testInt++;
            Debug.Log("started + " + callbackContext.BoolValue + ", duration: " + callbackContext.Duration + ", testBool: " + testBool + ", testInt: " + testInt);
        }

        public void Update(InputCallback callbackContext)
        {
            if (!testBool)
            {
                Debug.LogError("Failed to activate action, testBool is false.");
                return;
            }

            Debug.Log("Activated + " + callbackContext.FloatValue + ", duration: " + callbackContext.Duration + ", testBool: " + testBool + ", testInt: " + testInt);
            testInt++;
            testBool = false;
        }

        public void Cancel(InputCallback callbackContext)
        {
            if (!testBool)
            {
                Debug.LogError("Failed to cancel action, testBool is false.");
                return;
            }

            testInt++;
            Debug.Log("Cancelled + " + callbackContext.IntValue + ", duration: " + callbackContext.Duration + ", testBool: " + testBool + ", testInt: " + testInt);
        }

        public void Failed(InputCallback callbackContext)
        {
            Debug.LogError("Reset failed state, testBool is true.");
            testBool = true;
        }
    }
}
