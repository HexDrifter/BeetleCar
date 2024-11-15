using Beetle.InterfaceAdapters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Beetle.Framework
{
    public class PlayerGameplayUIView : BaseReactiveView
    {
        [SerializeField] private TextMeshProUGUI _text_shift_value;
        [SerializeField] private GaugeUIView _rpmGauge;
        private string[] _gears = {"R","N","1","2","3","4"};
        private PlayerViewModel _playerViewModel;

        public void SetModel(PlayerViewModel playerViewModel)
        {
            _playerViewModel = playerViewModel;

            _playerViewModel
                .Rpm
                .Subscribe((rpms) =>
                {
                    _rpmGauge.SetInputValue(rpms);
                })
                .AddTo(_disposables);

            _playerViewModel
                .Gear
                .Subscribe((gears) =>
                {
                    _text_shift_value.text = _gears[gears];
                })
                .AddTo(_disposables);
        }
    }

}
