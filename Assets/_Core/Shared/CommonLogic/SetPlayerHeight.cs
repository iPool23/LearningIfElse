using UnityEngine;

public class SetPlayerHeight : MonoBehaviour
{
    public float height = 1.3f;
    public bool editorOnly = true;

    void Update()
    {
        #if UNITY_EDITOR
        if (editorOnly)
        {
            Vector3 pos = transform.position;
            pos.y = height;
            transform.position = pos;
        }
        #endif
    }
}