using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        ApplyFix();
    }

    void Update()
    {
        // Forzar cada frame por si el SDK lo resetea
        if (cc.height != 1.8f)
            ApplyFix();
    }

    void ApplyFix()
    {
        cc.height = 1.8f;
        cc.center = new Vector3(0, 0.9f, 0);
        cc.radius = 0.3f;
    }
}