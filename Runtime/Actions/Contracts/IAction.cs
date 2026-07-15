#nullable enable

namespace HH.CompositionalActions.Contracts
{
    public interface IAction
    {
        string label { get; }

        /// <summary>Implementation of the generic trigger for actions. Should call the implementation methods.</summary>
        /// <param name="context">The context of how the action was called.</param>
        /// <param name="history">
        /// When the action is executed in a sequence it will receive an action result that stores all previous action
        /// results recursively</param>
        /// <returns>A result wrapper for the IEnumerator to run the action, and its implemented return values.</returns>
        IActionResult ExecuteAction(IActionContext context, IActionResult history);

        /// <summary>Copies the action and all its parameters and properties.</summary>
        /// <returns>A copy.</returns>
        IAction Copy();

        void UpdateLabel();
    }
}
