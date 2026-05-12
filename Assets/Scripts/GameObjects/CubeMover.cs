using UnityEngine;

namespace GameObjects
{
    public class CubeMover : MonoBehaviour
    {
        private float _time = 0f;
        
        private void Update()
        {
            _time += Time.deltaTime;
            
            Vector3 pos = transform.position;
            pos.y = 2* Mathf.Sin(_time);
            transform.position = pos;
        }
        
        public void Initialize(float initialValue)
        {
            _time = initialValue;
        }
    }
}
