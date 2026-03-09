using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonCommand : MonoBehaviour
{
    private Button _button;

    private Action _execute;
    private Func<bool> _canExecute;

    private readonly List<Action> _unsubscribeActions = new List<Action>();

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Init(Action execute, Func<bool> canExecute = null, params Func<Action, Action>[] subscribeToUpdateEvents)
    {
        _execute = execute;
        _canExecute = canExecute ?? (() => true);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Execute);

        if (subscribeToUpdateEvents != null)
        {
            foreach (var subscribe in subscribeToUpdateEvents)
            {
                var unsubscribe = subscribe.Invoke(Refresh);
                if (unsubscribe != null)
                    _unsubscribeActions.Add(unsubscribe);
            }
        }
        Refresh();
    }

    public bool CanExecute()
    {
        return _canExecute();
    }

    public void Execute()
    {
        if (!_canExecute())
            return;
        _execute();
    }

    private void Refresh()
    {
        if (_canExecute != null)
            _button.interactable = _canExecute();
    }

    private void OnDestroy()
    {
        foreach (var unsub in _unsubscribeActions)
            unsub.Invoke();
        _unsubscribeActions.Clear();
    }
}
