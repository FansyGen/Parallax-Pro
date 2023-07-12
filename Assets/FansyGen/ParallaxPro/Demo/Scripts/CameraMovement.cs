using UnityEngine;

namespace FansyGen.Demo
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;

        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            if (target == null)
            {
                Debug.LogWarning("Target not assigned! Please assign a target object.");
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0);
            transform.position = smoothedPosition;
        }
    }
}