using UnityEngine;
using System.Collections;
using System.Linq;

public class AnimationManager : MonoBehaviour {

    public AnimationBase[] updateComponents;
    public AnimationBase[] fixedUpdateComponents;

    void Start()
    {
        // remove duplicates from arrays
        updateComponents = updateComponents.Distinct().ToArray() as AnimationBase[];
        fixedUpdateComponents = fixedUpdateComponents.Distinct().ToArray() as AnimationBase[];
    }

    void Update()
    {
        int i;
        for (i = 0; i < updateComponents.Length; ++i)
        {
            updateComponents[i].OnUpdate();
        }
    }

    void FixedUpdate()
    {
        int i;
        for (i = 0; i < fixedUpdateComponents.Length; ++i)
        {
            fixedUpdateComponents[i].OnUpdate();
        }
    }
}
