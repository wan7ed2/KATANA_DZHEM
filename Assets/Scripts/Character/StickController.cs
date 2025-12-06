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

            var currentDir = _stickPigidbody.transform.right;
            var targetDir = (mouseWorld - _stickPigidbody.transform.position).normalized;

            var speed = _stickSettings.ROTATE_SPEED * Time.fixedDeltaTime;
            var currentRot = Quaternion.FromToRotation(Vector2.right, currentDir);
            var targetRot  = Quaternion.FromToRotation(Vector2.right, targetDir);

            var resultRot = Quaternion.RotateTowards(currentRot, targetRot, speed);
            _stickPigidbody.MoveRotation(resultRot);
        }
    }
}