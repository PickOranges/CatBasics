using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    private void Start()
    {
        name=depth.ToString();

        if(depth<=1) return;    

        Fractal child = Instantiate(this);
        child.depth = depth-1;
    }
}
