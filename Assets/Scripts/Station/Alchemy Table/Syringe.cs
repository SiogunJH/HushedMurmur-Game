using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField] Bird.Controller birdRepelled;
    [SerializeField] MeshRenderer fluidMeshRenderer;
    public bool IsFull { get; private set; } = true;
    Animator animator;

    const string T_USE = "Use";
    const string T_REFILL = "Refill";

    void Start()
    {
        SetComponentReferences();
        SetFluidColor(birdRepelled.repellantColor);
    }

    void SetComponentReferences()
    {
        animator = GetComponent<Animator>();
    }

    void SetFluidColor(Color color)
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        fluidMeshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        fluidMeshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void Use()
    {
        PlayerScreech.birdRepelled = birdRepelled.birdType;

        animator.SetTrigger(T_USE);
        IsFull = false;
    }

    public void Refill()
    {
        animator.SetTrigger(T_REFILL);
        IsFull = true;
    }
}
