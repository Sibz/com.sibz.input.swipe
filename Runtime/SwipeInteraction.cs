using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sibz.Input.Swipe
{

#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif
    public class SwipeInteraction : IInputInteraction
    {

        public float minDuration = 0.2f;
        public float maxDuration = 0.75f;

        private float startTime;

        public void Process(ref InputInteractionContext context)
        {
            if (context.isWaiting && context.ControlIsActuated())
            {
                context.Started();
                startTime = Time.time;
            }
            else if (context.isStarted && !context.ControlIsActuated())
            {
                var duration = Time.time - startTime;
                if (duration > minDuration &&
                    duration < maxDuration)
                {
                    // Interaction has been completed.
                    context.Performed();
                }
                else
                {
                    context.Canceled();
                }
            }
        }

        public void Reset()
        {
            startTime = 0;
        }

        static SwipeInteraction()
        {
            InputSystem.RegisterInteraction<SwipeInteraction>();
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init() { } // Trigger static constructor.

    }
}