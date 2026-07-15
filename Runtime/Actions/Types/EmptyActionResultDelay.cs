#nullable enable

namespace HH.CompositionalActions.Contracts
{
    using System.Collections;

    public class EmptyActionResultDelay : EmptyActionResult
    {
        public EmptyActionResultDelay(IEnumerator actionExecution)
        {
            Routine = actionExecution;
        }
    }
}
