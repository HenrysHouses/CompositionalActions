#nullable enable
namespace HH.CompositionalActions
{
    using HH.CompositionalActions.Contracts;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Actions/Action Sequence")]
    public class ActionSequenceScriptableObject : ScriptableObject
    {
        [SerializeField]
        private ActionSequence sequence = new ActionSequence();

        public bool IsValid => sequence.Actions.Count > 0;

        public ActionSequence GetSequence()
        {
            return sequence.Copy();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            sequence.UpdateElementNames();
        }
#endif
    }
}
