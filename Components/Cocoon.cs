using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.Components
{
    internal class Cocoon
    {
        private LaceBossScene _mainScene;
        private GameObject _cocoonObject;
        private Vector3 _initialPosition;
        private Animator? _animator;
        private float _fallDelay = 0.4f;
        private float _fallTime = 0.25f;

        public Cocoon(LaceBossScene scene, GameObject cocoonObject)
        {
            _cocoonObject = cocoonObject;
            _mainScene = scene;
        }  

        public void Init()
        {
            _cocoonObject.transform.SetPositionX(20);
            _initialPosition = _cocoonObject.transform.position;
            _animator = _cocoonObject.GetComponent<Animator>();
        }

        private void PlayAnimation(string animName)
        {
            if (_animator == null) { return; }
            _animator.Play(animName);
        }

        private IEnumerator FallDelay()
        {
            yield return new WaitForSeconds(_fallDelay);
            float timer = 0f;
            while (timer < _fallTime)
            {
                timer += Time.deltaTime;
                float fallAmount = (10 * Time.deltaTime) / _fallTime;
                _cocoonObject.transform.position -= new Vector3(0, fallAmount, 0);
                yield return new WaitForFixedUpdate();
            }
        }

        public void Fall()
        {
            _cocoonObject.transform.position = _initialPosition;
            PlayAnimation("weak_scream_to_ground");
            GameManager.instance.StartCoroutine(FallDelay());
        }
    }
}
