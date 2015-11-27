using UnityEngine;
using System.Collections;

namespace BnG.Editors
{

    public class TrackEditorGlobal
    {
        public static Transform[] currentSelection = new Transform[0];
    }

    /// <summary>
    /// Contains the user camera controls.
    /// </summary>
    public class CameraControls
    {
        public static InputCombination move = new InputCombination(KeyCode.LeftAlt, 2, true, E_INPUT.MOUSEANDMOD);
        public static InputCombination rotation = new InputCombination(KeyCode.LeftAlt, 0, true, E_INPUT.MOUSEANDMOD);
        public static InputCombination zoom = new InputCombination(KeyCode.LeftAlt, 1, true, E_INPUT.MOUSEANDMOD);
        public static InputCombination focus = new InputCombination(KeyCode.F, E_INPUT.KEYONLY);
    }

    /// <summary>
    /// Contains the user transform controls.
    /// </summary>
    public class TransformControls
    {
        public static InputCombination move = new InputCombination(KeyCode.Q, E_INPUT.KEYONLY);
        public static InputCombination rotate = new InputCombination(KeyCode.W, E_INPUT.KEYONLY);
        public static InputCombination scale = new InputCombination(KeyCode.E, E_INPUT.KEYONLY);
    }

    /// <summary>
    /// Contains data for input combinations.
    /// </summary>
    public class InputCombination
    {
        public InputCombination(KeyCode mod, KeyCode key, byte mouse, E_INPUT type) { modify = mod; keyCode = key; mouseButton = mouse; inputType = type; }
        public InputCombination(KeyCode mod, KeyCode key, E_INPUT type) { modify = mod; keyCode = key; inputType = type; }
        public InputCombination(KeyCode key, byte mouse, bool isMod, E_INPUT type)
        {
            if (isMod)
                modify = key;
            else
                keyCode = key;
            mouseButton = mouse;
            inputType = type;
        }
        public InputCombination(KeyCode key, E_INPUT type) { keyCode = key; inputType = type; }
        public InputCombination(byte mouse, E_INPUT type) { mouseButton = mouse; inputType = type; }

        public KeyCode modify = KeyCode.None;
        public KeyCode keyCode = KeyCode.None;
        public byte mouseButton = 255;
        public E_INPUT inputType;

        public bool GetInput()
        {
            switch(inputType)
            {
                case E_INPUT.KEYANDMOD:
                    return (Input.GetKey(modify) && Input.GetKeyDown(keyCode));
                case E_INPUT.KEYONLY:
                    return Input.GetKeyDown(keyCode);
                case E_INPUT.MOUSEANDKEY:
                    return (Input.GetKeyDown(keyCode) && Input.GetMouseButton(mouseButton));
                case E_INPUT.MOUSEANDMOD:
                    return (Input.GetKey(modify) && Input.GetMouseButton(mouseButton));
                case E_INPUT.MOUSEONLY:
                    return Input.GetMouseButton(mouseButton);
            }
            return false;
        }
    }

    public enum E_INPUT
    {
        MOUSEONLY,
        MOUSEANDKEY,
        MOUSEANDMOD,
        KEYONLY,
        KEYANDMOD
    }
}
