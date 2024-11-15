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
            float angle = value / limit;
            angle = -angle * (pointerMinAngle + pointerMaxAngle);
            angle += pointerMinAngle;

            if (pointerAngle >= angle)
            {
                pointerAngle -= pointerSpeed * Time.deltaTime;
            }
            else if (pointerAngle <= angle)
            {
                pointerAngle += pointerSpeed * Time.deltaTime;
            }
            SetPointerPosition(pointerAngle);
        }

        public void SetPointerPosition(float angle)
        {
            pointer.rectTransform.rotation = Quaternion.Euler(0f,0f, angle);
        }
    }

}