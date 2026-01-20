using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindData
{
    public void RequestSignIn(Action<DataBinding> onSuccess, Action onFail);

    public void SignOut(Action onSuccess);

}

public class DataBinding
{
    public string name;
    public string id;
    public string ava_url;
}
