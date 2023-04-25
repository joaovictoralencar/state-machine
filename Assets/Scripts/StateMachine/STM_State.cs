using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public abstract class STM_State<TAgentTemplate> where TAgentTemplate : STM_Agent<TAgentTemplate>
    {
        private bool _isRootState;
        private TAgentTemplate _agent;

        private STM_State<TAgentTemplate> _subState;
        private string _id;

        public string Id => _id;

        public STM_State<TAgentTemplate> SubState => _subState;

        public bool IsRootState => _isRootState;

        public TAgentTemplate Agent => _agent;

        protected abstract void OnEnterState();
        protected abstract void OnExitState();
        public abstract void SetSubStates();
        protected abstract void UpdateState();
        protected abstract void OnInitialize();
        protected abstract STM_State<TAgentTemplate> CheckSwitchState();

        public void Initialize(TAgentTemplate agent, bool isRootState, string key)
        {
            _agent = agent;
            _isRootState = isRootState;

            if (key != "")
                _id = key;
            else
            {
                _id = typeof(STM_State<TAgentTemplate>).ToString();
                if (agent.DebugStateMachine)
                    Debug.Log(Id, _agent.gameObject);
            }

            OnInitialize();
        }

        public void UpdateStates()
        {
            //If there's a state transition, do not update current states
            if (CheckSwitchState() != null) return;

            UpdateState(); //Update state

            //Update substate
            if (SubState == null) return;

            //If there's a state transition, do not update current substates
            if (SubState.CheckSwitchState() != null) return;
            SubState.UpdateStates();
        }

        public void ExitStates()
        {
            if (_agent.DebugStateMachine)
            {
                Debug.Log("Exiting state: " + _id, _agent.gameObject);
            }

            OnExitState();
            if (SubState == null) return;
            SubState.ExitStates();
            _subState = null;
        }

        public void EnterStates()
        {
            _agent.ChangeStateText(this);
            
            if (_agent.DebugStateMachine)
            {
                Debug.Log("Entering state: " + _id, _agent.gameObject);
            }

            OnEnterState();
            
            SubState?.EnterStates();
        }

        public void SetSubState(string subStateId)
        {
            if (!_isRootState || subStateId is "" or null || !Agent.ContainsState(subStateId)) return;

            STM_State<TAgentTemplate> newSubState = Agent.AllStates[subStateId];
            _subState = newSubState;


            if (_agent.DebugStateMachine)
            {
                Debug.Log("[" + _id + "] New sub state: " + newSubState.Id, _agent.gameObject);
            }
        }
    }
}