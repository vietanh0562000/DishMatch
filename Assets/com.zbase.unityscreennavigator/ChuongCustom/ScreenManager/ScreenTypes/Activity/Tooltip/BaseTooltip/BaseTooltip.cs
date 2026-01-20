namespace ChuongCustom.ScreenManager
{
    using DG.Tweening;
    using UnityEngine;

    public abstract class BaseTooltip : BaseActivity
    {
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _subBubble;

        private readonly Vector2 PIVOT_MID_TOP    = new Vector2(0.5f, 0f);
        private readonly Vector2 PIVOT_RIGHT_TOP  = new Vector2(0.1f, 0);
        private readonly Vector2 PIVOT_LEFT_TOP   = new Vector2(0.9f, 0);
        private readonly Vector2 PIVOT_RIGHT_DOWN = new Vector2(0.1f, 1);
        private readonly Vector2 PIVOT_MID_DOWN   = new Vector2(0.5f, 1);
        private readonly Vector2 PIVOT_LEFT_DOWN  = new Vector2(0.9f, 1);

        private readonly float widthTextPopup = 170;

        private Camera    _uiCamera;
        private Transform _rootTranform;
        private float     _percent;

        private bool _isOpen = false;

        private readonly bool _isInGame = false;

        protected override void Awake()
        {
            base.Awake();
            _uiCamera = Camera.main;
        }

        public void BindTransform(Transform rootTransform, float percent = 0.5f)
        {
            _rootTranform = rootTransform;
            _percent      = percent;

            OpenView();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
                CloseView();
        }

        protected override void CloseView()
        {
            HideToolTip();
            _isOpen = false;
            base.CloseView();
        }

        private void HideToolTip()
        {
            if (_bubble && _bubble.gameObject.activeSelf)
            {
                _bubble.DOScale(Vector3.zero, 0.1f).SetUpdate(true)
                    .OnComplete(() => { SetActive(false); });
            }
        }

        private void SetActive(bool isActive)
        {
            if (isActive)
            {
                OnOpenTooltip();
            }
            else
            {
                OnCloseTooltip();
            }
        }

        protected virtual void OnOpenTooltip()
        {
            _bubble.gameObject.SetActive(true);
            _subBubble.gameObject.SetActive(true);
            _bubble.DOKill();
            _bubble.transform.localScale = Vector3.one;
            _bubble.DOPunchScale(Vector3.one * 0.1f, 0.2f, 0, 0).SetUpdate(true);
        }

        protected virtual void OnCloseTooltip()
        {
            _bubble.gameObject.SetActive(false);
            _subBubble.gameObject.SetActive(false);
        }

        private void OpenView()
        {
            _isOpen = true;

            SetActive(true);

            var posOnScreen = _isInGame
                ? RectTransformUtility.WorldToScreenPoint(null, _rootTranform.position)
                : (Vector2)_uiCamera.WorldToScreenPoint(_rootTranform.position);

            bool onTopSide = OnTopSideOfScreen(posOnScreen, _percent);
            int  side      = CheckSideOfScreen(posOnScreen);

            //check to set pivot
            _bubble.pivot = side switch
            {
                0 => onTopSide ? PIVOT_MID_DOWN : PIVOT_MID_TOP,
                -1 => onTopSide ? PIVOT_RIGHT_DOWN : PIVOT_RIGHT_TOP,
                1 => onTopSide ? PIVOT_LEFT_DOWN : PIVOT_LEFT_TOP,
                _ => _bubble.pivot
            };

            var position = _rootTranform.position;
            _bubble.position    = position;
            _subBubble.position = position;

            var threshold = 50;

            float widthHalf = Screen.width * 0.5f;

            if (_bubble.anchoredPosition.x < -threshold &&
                widthHalf - _bubble.anchoredPosition.x < widthTextPopup + threshold)
            {
                _bubble.anchoredPosition -= Vector2.right * threshold;
            }
            else if (_bubble.anchoredPosition.x > threshold &&
                     widthHalf + _bubble.anchoredPosition.x <
                     widthTextPopup + threshold)
            {
                _bubble.anchoredPosition += Vector2.right * threshold;
            }

            _subBubble.rotation = Quaternion.Euler
            (Vector3.forward * (onTopSide ? 180 : 0)
             + Vector3.up * (side >= 0 ? onTopSide ? 0 : 180 : onTopSide ? 180 : 0));
        }

        private int CheckSideOfScreen(Vector2 posOnScreen)
        {
            if (posOnScreen.x < Screen.width * CheckLeft)
            {
                return -1;
            }

            if (posOnScreen.x > Screen.width * (1 - CheckLeft))
            {
                return 1;
            }

            return 0;
        }

        private bool OnTopSideOfScreen(Vector2 posOnScreen, float percent = 0.75f) { return posOnScreen.y > Screen.height * percent; }

        protected virtual float CheckLeft => 1 / 3f;
    }
}