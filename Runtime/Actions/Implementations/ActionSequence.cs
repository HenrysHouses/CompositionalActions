#nullable enable

namespace HH.CompositionalActions
{
    using System;
    using System.Collections.Generic;
    using HH.CompositionalActions.Contracts;
    using HH.Attributes;
    using UnityEngine;

    [Serializable]
    public class ActionSequence
    {
        [SerializeReference, ReferenceInstancer]
        private IAction[] order = Array.Empty<IAction>();

        [field:SerializeField] public string DisplayName { get; private set; } = string.Empty;

        public IReadOnlyList<IAction> Actions => Array.AsReadOnly(order);

        public IAction[] Order { get => order; set => order = value; }

        public IAction[] CopyActions()
        {
            IAction[] copies = new IAction[order.Length];

            for (int i = 0; i < order.Length; i++)
            {
                copies[i] = order[i].Copy();
            }

            return copies;
        }

        public ActionSequence Copy()
        {
            ActionSequence sequence = new ActionSequence
            {
                order = new IAction[order.Length],
                DisplayName = DisplayName,
            };

            for (int i = 0; i < order.Length; i++)
            {
                sequence.order[i] = order[i].Copy();
            }

            return sequence;
        }

#if UNITY_EDITOR
        public void UpdateElementNames()
        {
            foreach (IAction action in order)
            {
                action.UpdateLabel();
            }
        }
#endif
    }
}
