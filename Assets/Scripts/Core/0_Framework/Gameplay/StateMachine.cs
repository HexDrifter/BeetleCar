using UnityEngine;

namespace Beetle.Framework
{
    using Domain;
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    public class StateMachine : IStateMachine
    {
        [SerializeField] private IState _currentState;

        private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> _currentTransitions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();

        private static List<Transition> EmptyTransitions = new List<Transition>(0);

        public void Tick()
        {
            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);
            _currentState?.Tick();
        }

        public void PhysicsTick()
        {
            _currentState?.PhysicsTick();
        }

        public void SetState(IState state)
        {
            if (state == _currentState)
                return;
            _currentState?.OnExit();
            _currentState = state;

            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            if (_currentTransitions == null)
                _currentTransitions = EmptyTransitions;

            _currentState.OnEnter();
        }

        public string AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if(_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }
            var id = System.Guid.NewGuid().ToString();
            transitions.Add(new Transition(to, predicate, id));   
            return id;
        }

        public string AddAnyTransition(IState state, Func<bool> predicate)
        {
            var id = System.Guid.NewGuid().ToString();
            _anyTransitions.Add(new Transition(state, predicate, id));
            return id;
        }

        public void RemoveTransition(string id)
        {
            foreach (var transition in _transitions.Values)
            {
                for (int i = 0; i < transition.Count; i++)
                {
                    if (transition[i].ID == id)
                    {
                        transition.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private class Transition
        {
            public readonly string ID;
            public Func<bool> Condition { get; }
            public IState To { get; }

            public Transition(IState to, Func<bool> condition, string iD)
            {
                To = to;
                Condition = condition;
                ID = iD;
            }
        }

        private Transition GetTransition()
        {
            foreach(var transition in _anyTransitions)
                if( transition.Condition())
                    return transition;

            foreach(var transition in _currentTransitions)
                if( transition.Condition())
                    return transition;

            return null;
        }
    }
}
