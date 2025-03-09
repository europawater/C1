using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Material _mat;
    private float _distance;

    [Range(0f, 0.5f)]
    public float Speed = 0.0f;

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    private void LateUpdate()
    {
        _distance += Speed * Time.deltaTime;
        _mat.mainTextureOffset = new Vector2(_distance, 0);
    }
}
