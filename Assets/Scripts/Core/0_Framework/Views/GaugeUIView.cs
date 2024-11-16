using UnityEngine;
using UnityEngine.UI;
using Beetle.InterfaceAdapters;

namespace Beetle.Framework
{

    public class GaugeUIView : MonoBehaviour
    {


        [SerializeField] public Image dial;
        [SerializeField] public Image pointer;
        [SerializeField] public float pointerSpeed;
        [SerializeField] public float pointerAngle;
        [SerializeField] public float pointerMinAngle;
        [SerializeField] public float pointerMaxAngle;
        [SerializeField] public float limit;
        [SerializeField] private float _inputValue;
        public void Start()
        {
            pointerAngle = pointerMinAngle;
        }
        public void SetInputValue(float value)
        {
            _inputValue = value;
        }
        public void Update()
        {
            SetPointerValue(_inputValue);
        }
        public void SetPointerValue(float value)
        {
            float targetAngle = value / limit;
            targetAngle = -targetAngle * (pointerMinAngle + pointerMaxAngle);
            targetAngle += pointerMinAngle;

            float angleDifference = Mathf.Abs(pointerAngle - targetAngle);

            float adjustedSpeed = pointerSpeed * (angleDifference / (pointerMaxAngle + pointerMinAngle));
            if (pointerAngle >= targetAngle + 1f)
            {
                pointerAngle -= adjustedSpeed * Time.deltaTime;
            }
            else if (pointerAngle <= targetAngle - 1f)
            {
                pointerAngle += adjustedSpeed * Time.deltaTime;
            }
            SetPointerPosition(pointerAngle);
        }

        public void SetPointerPosition(float angle)
        {
            pointer.rectTransform.rotation = Quaternion.Euler(0f,0f, angle);
        }
    }

}