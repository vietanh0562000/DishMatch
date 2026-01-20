namespace BasePuzzle
{
    using UnityEngine;

    public static class WindowStyles
    {
        public static Color Red    = new Color(1f, 0.392f, 0.392f);
        public static Color Yellow = new Color(1f, 0.882f, 0.384f);
        public static Color Green = new Color(0.1843f, 0.8666f, 0.5725f);
        public static Color Blue = new Color(0.3294f, 0.549f, 1f);
        
        private static GUIStyle _textColor;
        public static GUIStyle TextColor(Color c)
        {
            if (_textColor == null)
            {
                _textColor = new GUIStyle(GUI.skin.label);
            }

            _textColor.normal.textColor = c;
            return _textColor;
        }
        
        private static GUIStyle _textCustomSize;
        public static GUIStyle TextCustomSize(int size)
        {
            if (_textCustomSize == null)
            {
                _textCustomSize = new GUIStyle(GUI.skin.label);
            }

            _textCustomSize.fontSize = size;
            return _textCustomSize;
        }
        
        private static GUIStyle _textColorCustomSize;
        public static GUIStyle TextColorCustomSize(Color c, int size)
        {
            if (_textColorCustomSize == null)
            {
                _textColorCustomSize = new GUIStyle(GUI.skin.label);
            }

            _textColorCustomSize.normal.textColor = c;
            _textColorCustomSize.fontSize         = size;
            return _textColorCustomSize;
        }

        private static GUIStyle _textCenter;
        public static GUIStyle TextCenter
        {
            get
            {
                if (_textCenter == null)
                {
                    _textCenter = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }

                return _textCenter;
            }
        }

        private static GUIStyle _textCenterCustomSize;
        public static GUIStyle TextCenterCustomSize(int size)
        {
            if (_textCenterCustomSize == null)
            {
                _textCenterCustomSize = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }

            _textCenterCustomSize.fontSize = size;
            return _textCenterCustomSize;
        }

        private static GUIStyle _textRight;
        public static GUIStyle TextRight
        {
            get
            {
                if (_textRight == null)
                {
                    _textRight = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleRight
                    };
                }

                return _textRight;
            }
        }

        private static GUIStyle _textRightCustomSize;
        public static GUIStyle TextRightCustomSize(int size)
        {
            if (_textRightCustomSize == null)
            {
                _textRightCustomSize = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleRight
                };
            }

            _textRightCustomSize.fontSize = size;
            return _textRightCustomSize;
        }

        private static GUIStyle _boldTextColor;
        public static GUIStyle BoldTextColor(Color c)
        {
            if (_boldTextColor == null)
            {
                _boldTextColor = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            _boldTextColor.normal.textColor = c;
            return _boldTextColor;
        }
        
        private static GUIStyle _boldTextColorCustomSize;
        public static GUIStyle BoldTextColorCustomSize(Color c, int size)
        {
            if (_boldTextColorCustomSize == null)
            {
                _boldTextColorCustomSize = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            _boldTextColorCustomSize.fontSize         = size;
            _boldTextColorCustomSize.normal.textColor = c;
            return _boldTextColorCustomSize;
        }

        private static GUIStyle _boldTextCenter;
        public static GUIStyle BoldTextCenter
        {
            get
            {
                if (_boldTextCenter == null)
                {
                    _boldTextCenter = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold
                    };
                }

                return _boldTextCenter;
            }
        }
        
        private static GUIStyle _boldTextColorCenter;
        public static GUIStyle BoldTextColorCenter(Color c)
        {
            if (_boldTextColorCenter == null)
            {
                _boldTextColorCenter = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }

            _boldTextColorCenter.normal.textColor = c;
            return _boldTextColorCenter;
        }

        private static GUIStyle _boldTextCenterCustomSize;
        public static GUIStyle BoldTextCenterCustomSize(int size)
        {
            if (_boldTextCenterCustomSize == null)
            {
                _boldTextCenterCustomSize = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }

            _boldTextCenterCustomSize.fontSize = size;
            return _boldTextCenterCustomSize;
        }
        
        private static GUIStyle _boldTextColorCenterCustomSize;
        public static GUIStyle BoldTextColorCenterCustomSize(Color c, int size)
        {
            if (_boldTextColorCenterCustomSize == null)
            {
                _boldTextColorCenterCustomSize = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }

            _boldTextColorCenterCustomSize.fontSize         = size;
            _boldTextColorCenterCustomSize.normal.textColor = c;
            return _boldTextColorCenterCustomSize;
        }
    }
}