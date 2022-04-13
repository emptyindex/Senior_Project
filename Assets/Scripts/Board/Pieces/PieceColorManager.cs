using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceColorManager : MonoBehaviour
{
    public List<Material> materials;

    private Material previousMaterial;

    public void SetMaterialAndRotation(bool isBlack)
    {
        Material material = null;

        if (isBlack)
        {
            material = materials.FirstOrDefault(m => m.name.Contains("Dark"));
            this.gameObject.transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        else
        {
            material = materials.FirstOrDefault(m => m.name.Contains("Light"));
        }

        previousMaterial = material;

        this.gameObject.GetComponent<Renderer>().material = material;
    }

    public void SetHighlight(bool enabled)
    {
        Material mat = previousMaterial;

        if (enabled)
        {
            mat = materials.FirstOrDefault(m => m.name.Contains("Highlight"));
        }

        this.gameObject.GetComponent<Renderer>().material = mat;
    }
}
