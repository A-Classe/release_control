﻿using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Option.Option3
{
    public class Option3Manager : UIManager
    {
        public enum Nav
        {
            Master,
            Music,
            Effect,
            Back
        }


        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private SliderController master;
        [SerializeField] private SliderController music;
        [SerializeField] private SliderController effect;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Master, master.rectTransform);
            cursor.AddPoint(Nav.Music, music.rectTransform);
            cursor.AddPoint(Nav.Effect, effect.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Master;

            SetState(Nav.Master);
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            if (isReset)
            {
                SetState(Nav.Master);
            }
        }

        /// <summary>
        ///     戻るボタンが押されたときに反映する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Clicked()
        {
            if (!current.HasValue)
            {
                return;
            }

            switch (current.Value)
            {
                case Nav.Back:
                    OnVolumeChanged?.Invoke(Nav.Master, master.Value);
                    OnVolumeChanged?.Invoke(Nav.Music, music.Value);
                    OnVolumeChanged?.Invoke(Nav.Effect, effect.Value);
                    OnBack?.Invoke();
                    break;
                default:
                    return;
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Master);
                return;
            }

            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                switch (current)
                {
                    case Nav.Master:
                        master.SetValue(master.Value + (direction.x > 0 ? 1 : -1));
                        OnVolumeChanged?.Invoke(Nav.Master, master.Value);
                        break;
                    case Nav.Music:
                        music.SetValue(music.Value + (direction.x > 0 ? 1 : -1));
                        OnVolumeChanged?.Invoke(Nav.Music, music.Value);
                        break;
                    case Nav.Effect:
                        effect.SetValue(effect.Value + (direction.x > 0 ? 1 : -1));
                        OnVolumeChanged?.Invoke(Nav.Effect, effect.Value);
                        break;
                    case Nav.Back:
                        back.OnPlay(() => OnBack?.Invoke());
                        break;
                }

                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    if (current.Value == Nav.Master)
                    {
                        return;
                    }

                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if (current.Value == Nav.Back)
                    {
                        return;
                    }

                    nextNav = current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }

        public event Action OnBack;

        public event Action<Nav, float> OnVolumeChanged;

        public void SetValues(int masterVol, int musicVol, int effectVol)
        {
            // initialize
            master.Setup(10f, 0f, 6f);
            music.Setup(10f, 0f, 10f);
            effect.Setup(10f, 0f, 10f);

            master.SetValue(masterVol);
            music.SetValue(musicVol);
            effect.SetValue(effectVol);
        }


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}