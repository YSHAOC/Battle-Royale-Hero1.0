using JKFrame;
using UnityEngine;
using UnityEngine.UI;
[UIElement(true, "UI/GameMainWindow", 1)]
public class UI_GameMainWindow : UI_WindowBase
{
    public ContainerUI action_Container; //可使用物品容器

    [SerializeField] private Text Name_Text; //名字
    [SerializeField] private Text Score_Text; //分数
    [SerializeField] private Image HPBar_Fill_Image; //血量
    [SerializeField] private Image DFBar_Fill_Image; //防御值
    [SerializeField] private Text BulletNum_Text; //子弹数

    //初始化
    public override void Init()
    {
        action_Container = GetComponentInChildren<ContainerUI>(); //可使用物品容器 组件获取

        Inventory_Manager.Instance.action_Container = action_Container; //容器管理器  赋值
    }
    public override void OnShow()
    {
        base.OnShow();
       action_Container.RefreshUI(); //这里要记得最开始的时候更新 不然 交换物品会报null
    }
    protected override void RegisterEventListener()
    {
        base.RegisterEventListener();
        EventManager.AddEventListener<string>("UpdateName", UpdateName);
        EventManager.AddEventListener<int>("UpdateHP", UpdateHP);
        EventManager.AddEventListener<int>("UpdateDF", UpdateDefence);
        EventManager.AddEventListener<int>("UpdateScore", UpdateScore);
        EventManager.AddEventListener<int>("Updatecurr_BulletNum", Updatecurr_BulletNum);
        EventManager.AddEventListener<int,int>("UpdateBullet", UpdateBullet);

    }

    protected override void CancelEventListener()
    {
        base.CancelEventListener();
        EventManager.RemoveEventListener<string>("UpdateName", UpdateName);
        EventManager.RemoveEventListener<int>("UpdateHP", UpdateHP);
        EventManager.RemoveEventListener<int>("UpdateDF", UpdateDefence);
        EventManager.RemoveEventListener<int>("UpdateScore", UpdateScore);
        EventManager.RemoveEventListener<int>("Updatecurr_BulletNum", Updatecurr_BulletNum);
        EventManager.RemoveEventListener<int, int>("UpdateBullet", UpdateBullet);

    }


    private void UpdateName(string name)
    {
        Name_Text.text = name;
    }
    private void UpdateHP(int hp)
    {
        HPBar_Fill_Image.fillAmount = hp / 100f;
    }
    private void UpdateDefence(int hp)
    {
        DFBar_Fill_Image.fillAmount = hp / 100f;
    }
    private void UpdateScore(int num)
    {
        Score_Text.text = num.ToString();
    }
    private void UpdateBullet(int curr,int max)
    {
        BulletNum_Text.text = curr + "/" + max;
    }
    private void Updatecurr_BulletNum(int curr)
    {
        BulletNum_Text.text = curr + "/" + 0;
    }
}
