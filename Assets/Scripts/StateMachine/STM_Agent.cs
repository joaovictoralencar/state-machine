using System;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class STM_Agent<TAgentTemplate> : MonoBehaviour where TAgentTemplate : STM_Agent<TAgentTemplate>
    {
        [SerializeField] private bool _debugStateMachine;
        private STM_State<TAgentTemplate> _currentState;

        private Dictionary<string, STM_State<TAgentTemplate>> _allStates =
            new Dictionary<string, STM_State<TAgentTemplate>>();

        public bool DebugStateMachine => _debugStateMachine;

        public Dictionary<string, STM_State<TAgentTemplate>> AllStates => _allStates;

        public STM_State<TAgentTemplate> SwitchState(string nextStateId)
        {
            if (nextStateId == "" || !ContainsState(nextStateId)) return null;


            //Get the state from map of states
            STM_State<TAgentTemplate> nextState = AllStates[nextStateId];

            //handle root states
            if (nextState.IsRootState)
            {
                if (_currentState != null)
                {
                    _currentState.ExitStates();
                    _currentState.SetSubState("");
                }

                //Force new state to have no child states
                nextState.SetSubState("");

                //Initialize Sub state
                nextState.SetSubStates();

                //Set new root state
                _currentState = nextState; //Change current state to be the new one
            }
            else if (_currentState != null && _currentState.SubState != null) //non root 
            {
                _currentState.SubState.ExitStates();
                _currentState.SetSubState(nextState.Id);
            }

            nextState.EnterStates();
            return nextState;
        }

        public bool ContainsState(string stateId, bool onlyChecks = false)
        {
            if (AllStates.ContainsKey(stateId)) return true;

            if (!onlyChecks)
                Debug.LogError(
                    "The State: " + stateId +
                    " is not registered. Check if you have already added it or check for spelling errors", gameObject);

            return false;
        }

        /// <summary>
        /// Initializes State Machine Agent
        /// </summary>
        /// <param name="initialState"> First state of the agent </param>
        public void InitializeAgent(STM_State<TAgentTemplate> initialState)
        {
            if (initialState == null)
            {
                Debug.LogError("Initial state is null");
                return;
            }

            AddState(initialState, true);

            if (DebugStateMachine)
                Debug.Log("First state of " + name + ": " + initialState.Id, gameObject);

            SwitchState(initialState.Id);
        }

        /// <summary>
        /// Adds a instanced state to the agent. Only adds with the was not added before and the state will only works if added correctly
        /// </summary>
        /// <param name="state">State instance</param>
        /// <param name="isRootState">Root states can only switch to root states</param>
        protected void AddState(STM_State<TAgentTemplate> state, bool isRootState)
        {
            //Initialize state dictionary if is null
            if (AllStates == null) _allStates = new Dictionary<string, STM_State<TAgentTemplate>>();

            //get stateId by class name
            string stateId = state.GetType().Name;

            //adding a state that already exist
            if (ContainsState(stateId, true))
            {
                if (DebugStateMachine)
                    Debug.LogWarning("Trying to add state " + stateId + " that was already been added.");
                return;
            }

            state.Initialize(this as TAgentTemplate, isRootState, stateId);
            AllStates.Add(stateId, state);
        }

        protected virtual void FixedUpdate()
        {
            if (_currentState == null) return;

            if (DebugStateMachine)
            {
                Debug.Log("Root State: " + _currentState.Id, gameObject);
                if (_currentState.SubState != null)
                    Debug.Log("Sub State: " + _currentState.SubState.Id, gameObject);
            }

            _currentState.UpdateStates();
        }

        #region Debug

        [SerializeField] private TextMeshProUGUI _rootStateText;
        [SerializeField] private TextMeshProUGUI _subStateText;

        public void ChangeStateText(STM_State<TAgentTemplate> state)
        {
            if (_debugStateMachine)
            {
                _rootStateText.gameObject.SetActive(true);
                _subStateText.gameObject.SetActive(true);
                if (state.IsRootState) _rootStateText.text = "RootState: " + state.Id;
                else _subStateText.text = "SubState: " + state.Id;
            }
            else
            {
                _rootStateText.gameObject.SetActive(false);
                _subStateText.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}