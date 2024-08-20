using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClownCar.Framework
{
    public class BaseReactiveView : MonoBehaviour
    {
        protected List<IDisposable> _disposables = new List<IDisposable>();

        public void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }
    }
}

