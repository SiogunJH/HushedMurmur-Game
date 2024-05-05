using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    [SerializeField]
    const string BOOL_SHADER_PROPERTY_OUTLINE_ENABLED = "_OutlineEnabled";

    [SerializeField]
    MeshRenderer meshRenderer;

    public virtual void Start()
    {
        if (meshRenderer == null) Debug.LogWarning($"Missing [Mesh Renderer] reference in [{gameObject.name}]");
    }

    public virtual void Focus()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(BOOL_SHADER_PROPERTY_OUTLINE_ENABLED, 1.0f);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public virtual void Unfocus()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(BOOL_SHADER_PROPERTY_OUTLINE_ENABLED, 0.0f);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public virtual void Interact()
    {
        Debug.LogError($"[{gameObject.name}] has no Interact() override");
    }
}
