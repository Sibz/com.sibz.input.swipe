using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;


namespace Sibz.Input.Swipe
{

#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif
    public class SwipeComposite : InputBindingComposite<Vector2>
    {

        [InputControl(layout = "Vector2")]
        public int Delta;

        [InputControl(layout = "Button")]
        public int Trigger;

        public float MinDistance = 10f;

        public enum ValueType
        {
            Delta,
            NormalisedDelta,
            OneUnitInt
        }
        public ValueType ReturnValueType = ValueType.Delta;

        private bool modifierActive = false;
        private Vector2 totalDelta;


        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            totalDelta += context.ReadValue<Vector2, Vector2MagnitudeComparer>(Delta);

            if (Mathf.Abs(totalDelta.x) < MinDistance && Mathf.Abs(totalDelta.y) < MinDistance)
            {
                return Vector2.zero;
            }

            switch (ReturnValueType)
            {
                case ValueType.Delta:
                    return totalDelta;
                case ValueType.OneUnitInt:
                    return new Vector2(totalDelta.x > 0 ? 1 : -1, totalDelta.y > 0 ? 1 : -1);
                case ValueType.NormalisedDelta:
                default:
                    return totalDelta.normalized;
            }
        }

        static SwipeComposite()
        {
            InputSystem.RegisterBindingComposite<SwipeComposite>();
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init() { } // Trigger static constructor.

        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            var modifierCurrentlyActive = context.ReadValueAsButton(Trigger);
            if (modifierActive && !modifierCurrentlyActive)
            {
                modifierActive = false;
            }
            else if (!modifierActive && modifierCurrentlyActive)
            {
                modifierActive = true;
                totalDelta = Vector2.zero;
            }

            if (modifierActive)
            {
                var val = ReadValue(ref context);
                if (val.x != 0 || val.y != 0)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}