using TMPro;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace ChuongCustom.ScreenManager.TestScene
{
    using UnityEngine;

    public class TestData : IScreenData
    {
        public int number;
    }

    [Popup("TestShowData", closeWhenClickOnBackdrop = false)]
    public class TestShowDataView : BasePopup<TestData>
    {
        public          TextMeshProUGUI numberText;
        protected override void Init(TestData data)
        {
            numberText.SetText(data.number.ToString());
        }
    }
}