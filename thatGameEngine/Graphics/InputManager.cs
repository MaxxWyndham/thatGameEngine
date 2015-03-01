using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Input;

namespace thatGameEngine
{
    //public enum KeyBinding
    //{
    //    KeysCameraSelect,
    //    KeysCameraPan,
    //    KeysCameraZoom,
    //    KeysCameraRotate,
    //    KeysCameraFrame,

    //    KeysActionScaleUp,
    //    KeysActionScaleDown,

    //    KeysClearSelection,
    //    KeysRenderMode,
    //    KeysCoordinateSystem
    //}

    public class InputManager
    {
        Dictionary<char, Action> bindings;
        Dictionary<string, char> bindingLookup;
        KeyboardState lastState;

        public static InputManager Current;

        public Dictionary<string, char> KeyboardBindings
        {
            get { return bindingLookup; }
        }

        public delegate void BindingsChangedHandler(object sender, EventArgs e);

        public event BindingsChangedHandler OnBindingsChanged;

        public InputManager()
        {
            bindings = new Dictionary<char, Action>();
            bindingLookup = new Dictionary<string, char>();

            Current = this;
        }

        public bool RegisterBinding(char key, string binding, Action action)
        {
            key = char.ToUpper(key);

            if (bindings.ContainsKey(key)) { return false; }

            bindings.Add(key, action);
            bindingLookup.Add(binding, key);

            return true;
        }

        //public void ReloadBindings()
        //{
        //    string[] b = new string[bindingLookup.Count];

        //    bindingLookup.Keys.CopyTo(b, 0);

        //    foreach (var binding in b)
        //    {
        //        var newKey = (char)Properties.Settings.Default[binding.ToString()];

        //        if (bindingLookup[binding] == newKey) { continue; }

        //        bindings[newKey] = bindings[bindingLookup[binding]];
        //        bindings.Remove(bindingLookup[binding]);
        //        bindingLookup[binding] = newKey;
        //    }

        //    if (OnBindingsChanged != null) { OnBindingsChanged(this, new EventArgs()); }
        //}

        public bool UpdateBinding(char oldKey, char newKey)
        {
            newKey = char.ToUpper(newKey);

            var binding = bindingLookup.Where(b => b.Value == oldKey).Select(b => b.Key).First();

            if (bindings.ContainsKey(newKey))
            {
                //Properties.Settings.Default[binding.ToString()] = oldKey;
                return false;
            }

            bindings[newKey] = bindings[oldKey];
            bindings.Remove(oldKey);
            bindingLookup[binding] = newKey;

            //Properties.Settings.Default[binding.ToString()] = newKey;

            return true;
        }

        public bool HandleInput(char KeyChar)
        {
            var key = char.ToUpper(KeyChar);

            if (bindings.ContainsKey(key))
            {
                bindings[key]();

                return true;
            }
            else
            {
                return false;
            }
        }

        public KeyboardShortcuts GetKeyboardShortcuts()
        {
            return new KeyboardShortcuts();
        }

        // From ViewportManager
        //public void UpdateKeyboardMovement()
        //{
        //    //if (!isMouseDown) { return; }

        //    var state = Keyboard.GetState();
        //    float dt = SceneManager.Current.DeltaTime;

        //    if (active.ProjectionMode == Projection.Orthographic)
        //    {
        //        if (state[Key.W]) { active.Camera.MoveCamera(Camera.Direction.Up, dt); }
        //        if (state[Key.S]) { active.Camera.MoveCamera(Camera.Direction.Down, dt); }

        //        if (state[Key.A]) { active.Camera.MoveCamera(Camera.Direction.Left, dt); }
        //        if (state[Key.D]) { active.Camera.MoveCamera(Camera.Direction.Right, dt); }
        //    }
        //    else
        //    {
        //        if (state[Key.A]) { active.Camera.MoveCamera(Camera.Direction.Left, dt); }
        //        if (state[Key.D]) { active.Camera.MoveCamera(Camera.Direction.Right, dt); }

        //        if (state[Key.W]) { active.Camera.MoveCamera(Camera.Direction.Forward, dt); }
        //        if (state[Key.S]) { active.Camera.MoveCamera(Camera.Direction.Backward, dt); }

        //        if (state[Key.Z]) { active.Camera.MoveCamera(Camera.Direction.Up, dt); }
        //        if (state[Key.X]) { active.Camera.MoveCamera(Camera.Direction.Down, dt); }

        //        if (state[Key.Q]) { active.Camera.Rotate(0, 0, -dt * 50); }
        //        if (state[Key.E]) { active.Camera.Rotate(0, 0, dt * 50); }

        //        if (state[Key.Keypad4]) { active.Camera.Rotate(dt, 0, 0); }
        //        if (state[Key.Keypad6]) { active.Camera.Rotate(-dt, 0, 0); }
        //        if (state[Key.Keypad2]) { active.Camera.Rotate(0, dt, 0); }
        //        if (state[Key.Keypad8]) { active.Camera.Rotate(0, -dt, 0); }
        //        if (state[Key.Keypad7]) { active.Camera.Rotate(0, 0, dt); }
        //        if (state[Key.Keypad9]) { active.Camera.Rotate(0, 0, -dt); }

        //        if (state[Key.Keypad1]) { active.Camera.MoveCamera(Camera.Direction.Left, dt); }
        //        if (state[Key.Keypad3]) { active.Camera.MoveCamera(Camera.Direction.Right, dt); }
        //    }
        //}
    }

    public partial class KeyboardShortcuts
    {
    }
}
