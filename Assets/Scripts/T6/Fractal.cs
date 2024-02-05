using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    private void Start()
    {
        name=depth.ToString();

        if(depth<=1) return;    

        // create Child 1
        Fractal child = Instantiate(this);
        child.depth = depth-1;
        // assign the parent's TF to the child, and then change it to correct position.
        child.transform.SetParent(transform, false);
        child.transform.localPosition = 0.75f * Vector3.right; // decrease the distance and shrink the size of ball each recursion
        child.transform.localScale = 0.5f * Vector3.one;

        // create Child 2
        child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.SetParent(transform, false);
        child.transform.localPosition = 0.75f * Vector3.up;
        child.transform.localScale = 0.5f * Vector3.one;
    }
}
