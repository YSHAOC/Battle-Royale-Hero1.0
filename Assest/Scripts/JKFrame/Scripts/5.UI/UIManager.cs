using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JKFrame
{
    public class UIManager : ManagerBase<UIManager>
    {

        #region 内部类

        //主要为该Layer下的Mask遮挡的设置 （位置信息，射线遮挡）
        [Serializable]
        private class UILayer
        {
            public Transform root;   //Layer0/1/2/3/4/
            public Image maskImage;  //Mask(1)/(2)/(3)/(4)
            private int count = 0;
            public void OnShow()
            {
                count += 1; //Mask射线遮挡为true
                Update(); //更新Mask遮挡
            }

            public void OnClose()
            {
                count -= 1; //Mask射线遮挡为false
                Update(); //更新Mask遮挡
            }

            //这里是设置（Mask遮挡）的位置更新
            private void Update()
            {
                //这里是遮挡射线相关的
                maskImage.raycastTarget = count != 0; //如果count不等于0（OnShow显示）就true -- 遮挡 ; 如果count等于0(OnClose不显示)就false -- 不遮挡

                //公式：（ 生成的位置索引 = 父类的子物体数量 - 减去想要的倒数第几个 ）
                //ps：我想要永远在倒数第二位置 ：transform.SetSiblingIndex（父类的子物体数量 - 想要的倒数第二）
                //这里是位置相关的
                int posIndex = root.childCount - 2; //该Layer中子物体的倒数第2个
                maskImage.transform.SetSiblingIndex(posIndex < 0 ? 0 : posIndex); //设置Mask的位置信息
            }
        }
        #endregion


        /// <summary>
        /// 元素资源库
        /// </summary>
        public Dictionary<Type, UIElement> UIElementDic { get { return GameRoot.Instance.GameSetting.UIElementDic; } }
        private const string TipsLocalizationPackName = "Tips";

        [SerializeField]
        private UILayer[] UILayers;


        #region 这是关于 Tip
        // 提示窗
        [SerializeField]
        private UITips UITips;
        public void AddTips(string info)
        {
            UITips.AddTips(info);
        }
        public void AddTipsByLocailzation(string tipsKeyName)
        {
            UITips.AddTips(LocalizationManager.Instance.GetContent<L_Text>(TipsLocalizationPackName, tipsKeyName).content);
        }
        #endregion


        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public T Show<T>(int layer = -1) where T : UI_WindowBase
        {
            return Show(typeof(T), layer) as T;
        }
        public UI_WindowBase Show(Type type, int layer = -1)
        {
            if (UIElementDic.ContainsKey(type))
            {
                UIElement info = UIElementDic[type];
                int layerNum = layer == -1 ? info.layerNum : layer; //如果是-1（也就是没设置）就选择GameSetting里的UI层级;

                // 实例化实例或者获取到实例，保证窗口实例存在
                if (info.objInstance != null)
                {
                    info.objInstance.gameObject.SetActive(true);  //显示
                    info.objInstance.transform.SetParent(UILayers[layerNum].root);  //设置父物体
                    info.objInstance.transform.SetAsLastSibling();
                    info.objInstance.OnShow(); //主要进行事件的注册
                }
                else
                {
                    //基于预制体实例化
                    UI_WindowBase window = ResManager.InstantiateForPrefab(info.prefab, UILayers[layerNum].root).GetComponent<UI_WindowBase>();
                    info.objInstance = window;
                    window.Init();  //窗口的初始化
                    window.OnShow(); //主要进行事件的注册
                }
                info.layerNum = layerNum;

                //对 （该Layer）的 （Mask遮挡） 设置
                UILayers[layerNum].OnShow();

                return info.objInstance;
            }
            // 资源库中没有意味着不允许显示
            return null;
        }




        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        public void Close<T>()
        {
            Close(typeof(T));
        }
        public void Close(Type type)
        {
            if (UIElementDic.ContainsKey(type))
            {
                UIElement info = UIElementDic[type];
                if (info.objInstance == null) return;

                info.objInstance.OnClose(); //主要是取消事件
                // 缓存则隐藏
                if (info.isCache)
                {
                    info.objInstance.transform.SetAsFirstSibling();
                    info.objInstance.gameObject.SetActive(false);
                }
                // 不缓存则销毁
                else
                {
                    Destroy(info.objInstance);
                    info.objInstance = null;
                }
                UILayers[info.layerNum].OnClose();
            }
        }




        /// <summary>
        /// 关闭全部窗口
        /// </summary>
        public void CloseAll()
        {
            // 处理缓存中所有状态的逻辑
            var enumerator = UIElementDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.objInstance != null
                    && enumerator.Current.Value.objInstance.gameObject.activeInHierarchy == true)
                {
                    enumerator.Current.Value.objInstance.Close();
                }
            }
        }
    }
}