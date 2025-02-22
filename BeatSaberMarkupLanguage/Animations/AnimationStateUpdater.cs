﻿using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Animations
{
    public class AnimationStateUpdater : MonoBehaviour
    {
        public Image image;

        private AnimationControllerData _controllerData;

        public AnimationControllerData controllerData
        {
            get => _controllerData;
            set
            {
                if (_controllerData != null)
                {
                    OnDisable();
                }

                _controllerData = value;

                if (isActiveAndEnabled)
                {
                    OnEnable();
                }
            }
        }

        private void OnEnable()
        {
            controllerData?.activeImages.Add(image);
        }

        private void OnDisable()
        {
            controllerData?.activeImages.Remove(image);
        }

        private void OnDestroy()
        {
            controllerData?.activeImages.Remove(image);
        }
    }
}
