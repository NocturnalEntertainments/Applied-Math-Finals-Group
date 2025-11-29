using UnityEngine;

public class MeshCulling : MonoBehaviour
{
    public static Vector3 GetCulledScale(Vector3 objPos, Vector3 originalScale, Transform camera, float maxDistance = 100f)
    {
        Vector3 toObj = objPos - camera.position;
        float dot = Vector3.Dot(camera.forward, toObj.normalized);
        float distance = toObj.magnitude;

        // If in front of camera and within maxDistance, keep scalee
        if (dot > 0f && distance <= maxDistance)
        {
            return originalScale;
        }

        //hide the object heree
        return Vector3.zero;
    }
}
