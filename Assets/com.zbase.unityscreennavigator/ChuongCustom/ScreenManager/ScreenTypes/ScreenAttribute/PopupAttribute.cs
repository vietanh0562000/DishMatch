namespace ChuongCustom
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class PopupAttribute : Attribute
    {
        public string namePath;
        public bool   loadAddressable;
        public bool   closeWhenClickOnBackdrop;
        public bool   showAnim;

        public PopupAttribute(string namePath, bool loadAddressable = true, bool closeWhenClickOnBackdrop = false, bool showAnim = true)
        {
            this.namePath                 = namePath;
            this.loadAddressable          = loadAddressable;
            this.closeWhenClickOnBackdrop = closeWhenClickOnBackdrop;
            this.showAnim                 = showAnim;
        }
    }
}