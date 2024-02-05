using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    private void Start()
    {
        name=depth.ToString();

        if(depth<=1) return;

        // S1. create children 
        Fractal childA = CreateChild(Vector3.up);
        Fractal childB = CreateChild(Vector3.right);
        // S2. bind them to the parent
        childA.transform.SetParent(transform, false);
        childB.transform.SetParent(transform, false);
        // Note:
        // We don't bind to parent immediately, because that will change the hiarchy of the parent,
        // and by the time of creating the 2nd child, it will take both parent and its 1st child as the reference,
        // thus the result is wrong.
        // Solution: create all children, then bind them to parent one-by-one.
    }

    Fractal CreateChild(Vector3 direction)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.localPosition = 0.75f * direction;
        child.transform.localScale = 0.5f * Vector3.one;
        return child;
    }
}
