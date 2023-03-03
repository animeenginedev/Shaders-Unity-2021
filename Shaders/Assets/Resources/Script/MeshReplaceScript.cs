using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshReplaceScript : MonoBehaviour
{

    public Material change_to_material;
    Material current_material = null;

    void Update()
    {
        if(current_material != change_to_material)
        {
            current_material = change_to_material;

            var meshes = transform.GetComponentsInChildren<MeshRenderer>();
            foreach(var mesh in meshes)
            {
                mesh.material = current_material;
            }                
        }
    }
}
