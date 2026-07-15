#nullable enable
namespace HH.CompositionalActions
{
    using HH.CompositionalActions.Contracts;
    using HH.Attributes;
    using UnityEngine;
    using static ActionSequenceScriptableObject;

    [CreateAssetMenu(menuName = "Actions/Actor Action Sequence")]
    public class ActorActionSequenceScriptableObject : ScriptableObject
    {
        [SerializeField, ReferenceInstancerOverride(typeof(IActorAction))]
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
