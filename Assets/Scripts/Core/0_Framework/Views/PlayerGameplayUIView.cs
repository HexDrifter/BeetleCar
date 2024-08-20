using ClownCar.InterfaceAdapters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ClownCar.Framework
{
    public class PlayerGameplayUIView : BaseReactiveView
    {
        [SerializeField] private TextMeshProUGUI _text_rpm_value;
        [SerializeField] private TextMeshProUGUI _text_shift_value;
        [SerializeField] private Slider          _slider_value;
        private string[] _gears = {"R","N","1","2","3","4"};
        private PlayerViewModel _playerViewModel;

        public void SetModel(PlayerViewModel playerViewModel)
        {
            _playerViewModel = playerViewModel;

            _playerViewModel
                .Rpm
                .Subscribe((rpms) =>
                {
                    _text_rpm_value.text = "RPM: " + rpms.ToString("N1");
                    _slider_value.value  = Mathf.Clamp(rpms/5000f,0.1f,1f);
                })
                .AddTo(_disposables);

            _playerViewModel
                .Gear
                .Subscribe((gears) =>
                {
                    _text_shift_value.text = "Gear: "+ _gears[gears];
                })
                .AddTo(_disposables);
        }
    }

}
