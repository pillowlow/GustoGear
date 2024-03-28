using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction; // 添加 Oculus.Interaction 的命名空间

public class GrabAndCallFunction : MonoBehaviour
{
    [System.Serializable]
    public class FunctionEvent : UnityEvent<GameObject> { } // 定义一个 UnityEvent，带有一个 GameObject 参数

    public PointableUnityEventWrapper pointableEventWrapper; // 引用 PointableUnityEventWrapper 组件

    public FunctionEvent functionToCall; // 用于调用的事件

    private void Start()
    {
        // 添加监听器
        pointableEventWrapper.WhenSelect.AddListener(OnSelect);
    }

    // 选择事件处理函数
    private void OnSelect(PointerEvent evt)
    {
        // 调用在 Inspector 中选择的函数
        functionToCall.Invoke(gameObject);
    }
}
