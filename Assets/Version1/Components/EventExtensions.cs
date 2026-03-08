using System;

public static class EventExtensions
{
    /// <summary>
    /// Subscribes to an event of any type, ignoring the event parameters,
    /// and returns an Action that will unsubscribe.
    /// </summary>
    public static Action SubscribeIgnoringParameters<TEventArgs>(
        Action<EventHandler<TEventArgs>> add,
        Action<EventHandler<TEventArgs>> remove,
        Action callback
    )
    {
        void handler(object sender, TEventArgs args) => callback();
        add(handler);
        return () => remove(handler);
    }

    public static Action SubscribeIgnoringParameters(
        Action<Action> add,
        Action<Action> remove,
        Action callback
    )
    {
        void handler() => callback();
        add(handler);
        return () => remove(handler);
    }
}
