using Unity.VisualScripting;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    int depth = 4;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    FractalPart[][] parts;
    GameObject[][] gos;

    static Vector3[] directions = {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    struct FractalPart
    {
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    FractalPart CreatePart(int levelIndex, int childIndex, float scale)
    {
        var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;
        go.transform.localScale = scale * Vector3.one;

        return new FractalPart
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform
        };
    }

    void Awake()
    {
        // S1. create a vector
        parts = new FractalPart[depth][ ];
        gos = new GameObject[depth][];
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
        {
            parts[i] = new FractalPart[length];
            gos[i]= new GameObject[length];
        }

        // S2. then fill it
        float scale = 1f;
        parts[0][0]=CreatePart(0, 0, scale);       // root node is NOT a child of any node.

        for (int li = 1; li < parts.Length; li++)  // level idx
        {
            scale *= 0.5f;
            FractalPart[] levelParts = parts[li];   // take the init. memory blocks and fill it
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)  // first place idx
            {
                for (int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi+ci]=CreatePart(li, ci, scale);
                }
            }
            // Why we don't need this line?
            // Because all C# vector/array are on the heap, thus this operation is the same as shallow copy in C++.
            // Means that if you change the new ref' value, you also change the original data, because there was only one copy of data on heap.
            //parts[li] = levelParts;  

        }
    }

    void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
        FractalPart rootPart = parts[0][0];

        // ???? should NOT be left-multiplication ???
        // Because the deltaRotation is actually the very first local rotation !!!!
        // Here is the rotation chain: deltaRotation-->rotation-->parent rotation.
        // But since this is the root node, it doesn't have any parent, thus we can neglet the third rotation.
        rootPart.rotation *= deltaRotation;
        rootPart.transform.localRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        // Root node cannot move(actually it can rotate, but that will make no difference).
        // Thus we don't have to update root node.
        for (int li = 1; li < parts.Length; li++)
        {
            FractalPart[] levelParts = parts[li];
            FractalPart[] parentParts = parts[li - 1];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                Transform parentTransform = parentParts[fpi / 5].transform;  // This parent node idx calculation is the same as binary tree.
                FractalPart part = levelParts[fpi];
                // ???? should NOT be left-multiplication ???
                // Because the deltaRotation is actually the very first local rotation !!!!
                part.rotation *= deltaRotation; 

                part.transform.localRotation =
                    parentTransform.localRotation * part.rotation;

                //part.rotation = parentTransform.localRotation * deltaRotation * part.rotation;

                part.transform.localPosition = 
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                    (1.5f * part.transform.localScale.x * part.direction);
                levelParts[fpi] = part;
            }
        }
    }
}
