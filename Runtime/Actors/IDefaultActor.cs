#nullable enable
namespace HH.Actors.Contracts
{
    using UnityEngine;
    using static HH.Actors.Contracts.ActorCallback;

    public interface IDefaultActor
    {
        ActorCallbackSource CallbackSource { get; }
    }

    public interface IGameObjectActor : IDefaultActor
    {
        GameObject GetGameObject();
    }

    public class NoActor : IDefaultActor
    {
        ActorCallbackSource IDefaultActor.CallbackSource => throw new System.NullReferenceException($"Attempted to get the action callbacks from a NoActor.");
    }
}
