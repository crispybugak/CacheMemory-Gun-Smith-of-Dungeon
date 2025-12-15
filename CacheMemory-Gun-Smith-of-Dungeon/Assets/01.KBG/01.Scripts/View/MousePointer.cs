using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _01.KBG._01.Scripts.View
{
    public class MousePointer : MonoBehaviour
    {
        [SerializeField] private AgentMovementSO agentMovement;

        public float recoilTime = 0.2f;
        [SerializeField] private Ease ease = Ease.OutCubic;
        public float recoverySpeed;
        
        public RectTransform rectTransform {get; private set;}
        
        private Vector2 mousePos => agentMovement.mouseDir;
        [SerializeField] private Vector2 recoveryPos;
        private Vector2 currentPos => mousePos + recoveryPos;

        private void Awake()
        {
            rectTransform =  GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void FixedUpdate()
        {
            
            if (!isrecoiling)
            {
                Vector2 dir = (-recoveryPos).normalized;
                if (recoverySpeed * 1 >= recoveryPos.magnitude)
                {
                    recoveryPos = Vector2.zero;
                }
                else
                {
                    recoveryPos += dir * (recoverySpeed * 1);
                }
                
                moveStartPos = recoveryPos;
            }

            rectTransform.position = currentPos;
        }

#if UNITY_EDITOR
        [SerializeField] private Vector2 testDir;
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                AddRecoil(testDir);
            }
        }
#endif
        private TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore;
        private Vector2 targetDir = Vector2.zero;
        private bool isrecoiling = false;
        private Vector2 moveStartPos;
        public void AddRecoil(Vector2 dir)
        {
            targetDir += dir;
            isrecoiling = true;
            tweenerCore.Kill();
            tweenerCore = DOTween.To(() => recoveryPos, x => recoveryPos = x, moveStartPos + targetDir, recoilTime)
                .SetEase(ease)
                .OnComplete(AddRecoilEnd);
            
            
        }

        private void AddRecoilEnd()
        {
            targetDir = Vector2.zero;
            moveStartPos = recoveryPos;
            isrecoiling = false;

        }
    }
}