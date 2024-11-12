using Beetle.Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle.InterfaceAdapters
{
    public class PlayerPresenter : PlayerRpmOutput, PlayerGearOutput
    {
        private readonly PlayerViewModel _playerViewModel;
        public PlayerPresenter(PlayerViewModel playerViewModel)
        {
            _playerViewModel = playerViewModel;
        }
        public void ShowRPM(float rpm)
        {
            _playerViewModel.Rpm.Value = rpm;
        }
        public void ShowGear(int gear)
        {
            _playerViewModel.Gear.Value = gear;
        }
    }
}
