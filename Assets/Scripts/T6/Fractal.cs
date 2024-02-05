using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    private void Start()
    {
        name = depth.ToString();

        if(depth<=1) return;

        // S1. create children 
        Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
        Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
        Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
        Fractal childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
        Fractal childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));
        // S2. bind them to the parent
        childA.transform.SetParent(transform, false);
        childB.transform.SetParent(transform, false);
        childC.transform.SetParent(transform, false);
        childD.transform.SetParent(transform, false);
        childE.transform.SetParent(transform, false);
        // Note:
        // We don't bind to parent immediately, because that will change the hiarchy of the parent,
        // and by the time of creating the 2nd child, it will take both parent and its 1st child as the reference,
        // thus the result is wrong.
        // Solution: create all children, then bind them to parent one-by-one.
    }

    void Update()
    {
        transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
    }

    Fractal CreateChild(Vector3 direction, Quaternion rotation)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = rotation;
        child.transform.localScale = 0.5f * Vector3.one;
        return child;
    }
}
