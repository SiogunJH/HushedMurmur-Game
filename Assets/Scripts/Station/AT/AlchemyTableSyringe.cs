using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyTableSyringe : MonoBehaviour
{
    [SerializeField] Bird.Type birdRepelled;
    [SerializeField] AudioClip customScreechNoise;
    [SerializeField] Color color;

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void OnUse()
    {
        PlayerScreech.birdRepelled = birdRepelled;
        Destroy(gameObject);
    }
}
