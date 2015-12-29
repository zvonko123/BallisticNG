using UnityEngine;
using System.Collections;

public class AnimationMulti : AnimationBase {

    public AnimationBase[] components;

    public override void OnUpdate()
    {
        int i;
        for (i = 0; i < components.Length; ++i)
            components[i].OnUpdate();
    }
}
