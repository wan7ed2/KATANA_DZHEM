using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public class StickController
    {
        public StickController(InputSystem_Actions.PlayerActions input, Rigidbody2D stickPigidbody, StickSettings stickSettings)
        {
            _input = input;
            _stickPigidbody = stickPigidbody;
            _stickSettings = stickSettings;
        }

        public void Start()
        {
            
        }
        
        public void Stop()
        {
            
        }

        public void Tick()
        {
            RotateStick();
        }

        private InputSystem_Actions.PlayerActions _input;
        private Rigidbody2D _stickPigidbody;
        private StickSettings _stickSettings;

        private void RotateStick()
        {
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            var targetDir = (mouseWorld - _stickPigidbody.transform.position).normalized;

            var speed = _stickSettings.ROTATE_SPEED * Time.fixedDeltaTime;
            var targetRot  = Quaternion.FromToRotation(Vector2.right, targetDir);

            var current = _stickPigidbody.transform.rotation * Quaternion.identity;
            var delta = targetRot * Quaternion.Inverse(current);

            if (delta.w < 0f)
            {
                delta.x = -delta.x;
                delta.y = -delta.y;
                delta.z = -delta.z;
                delta.w = -delta.w;
            }

            delta.ToAngleAxis(out float angleDeg, out Vector3 axis);
            var angleRad = angleDeg * Mathf.Deg2Rad;

            var torque =
                angleRad * axis.z * speed
                - _stickPigidbody.angularVelocity * _stickSettings.DAMPING;

            _stickPigidbody.AddTorque(torque, ForceMode2D.Force);
        }
    }
}