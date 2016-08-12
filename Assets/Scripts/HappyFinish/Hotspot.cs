using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
//using HappyFinish.Project;

namespace HappyFinish.VR
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class Hotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        
        public enum InteractionType
        {
            Timer,
            Click
        }
        [Header("Hotspot type")]
        public InteractionType interactionType;


        [Header("Hotspot timer")]
        public float triggerTime = 1.5f;

        [Header("Transition")]
        public float fadeInTime = 1.5f;
        public float fadeOutTime = 1.5f;

        public float Value
        {
            get
            {

                return Mathf.Clamp01(m_CurrentTime / triggerTime);
            }
        }

        //[Header("Transition")]
        //public float fadeInTime = 1.5f;
        //public float fadeOutTime = 1.5f;

        public bool IsVisible { get; private set; }

        private bool pointerIn;

        protected virtual void OnAwake()
        {

        }
        protected virtual void OnStart()
        {

        }


        float m_CurrentTime = 0f;

        void Awake()
        {
            IsVisible = true;
            OnAwake();
            //Debug.Log("Awake : " + name);
        }

        void Start()
        {
            //if (fadeInTime > triggerTime)
            //    fadeInTime = triggerTime;

            pointerIn = false;
            OnStart();
            //Debug.Log("Start : " + name);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.LogWarning("OnPointerEnter isVisible: " + IsVisible + " time: " + Time.time);
            if (!IsVisible)
                return;

            Enter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_CurrentTime = 0;
            //Debug.LogWarning("OnPointerExit isVisible: " + IsVisible);
            if (!IsVisible)
                return;

            Exit(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsVisible)
                return;
            m_CurrentTime = 0f;
            ClickBehaviour();
        }

        void Enter(PointerEventData eventData)
        {
            pointerIn = true;

            if (interactionType == InteractionType.Timer)
            {
                StartCoroutine(OnPointerStay(eventData));
            }
            else
            {
                StartCoroutine(UpdatePointerStay());
            }

            EnterBehaviour();
        }

        void Exit(PointerEventData eventData)
        {
            m_CurrentTime = 0;
            pointerIn = false;

            StopAllCoroutines();
            ExitBehaviour();
        }

        private IEnumerator OnPointerStay(PointerEventData eventData)
        {
            m_CurrentTime = 0;
            while (m_CurrentTime < triggerTime)
            {
                m_CurrentTime += Time.deltaTime;
                StayBehaviour();
                yield return new WaitForSeconds(Time.deltaTime / triggerTime);
                //SetSliderValue(currentTime / triggerTime);
            }
            //Debug.Log(name + ": Finish | " + (m_CurrentTime < triggerTime));
            m_CurrentTime = 0;

            // Timer ended simulates a click and the OnClick UI Button event is called
            //ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            //ClickBehaviour();
            Click(false);
            
        }

        private IEnumerator UpdatePointerStay()
        {
            while (pointerIn)
            {
                //Waiting for input
                //if (InputManager.Instance.Input1)
                {
                    pointerIn = false;
                    ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    //ClickBehaviour();
                    //Click();
                    yield break;
                }

                yield return null;
            }
        }

        private void Click(bool TriggerSubclassBehaviour)
        {
            m_CurrentTime = 0f;
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            if (TriggerSubclassBehaviour)
            {
                ClickBehaviour();
            }
        }














        
        //
        protected abstract void EnterBehaviour();
        protected abstract void ExitBehaviour();
        protected abstract void StayBehaviour();
        protected abstract void ClickBehaviour();
    }
}