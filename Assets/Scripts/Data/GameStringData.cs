using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GameString
{
    [SerializeField]
    string stringT;
    [SerializeField]
    string stringS;

    public void init( string str )
    {
        stringS = str;
        stringT = ChineseStringUtility.ToTraditional( str );
    }

    public string String
    {
        get
        {
            switch ( GameSetting.instance.location )
            {
                case GameSetting.GameLocation.SimplifiedChinese:
                    return stringS;
                case GameSetting.GameLocation.TraditionalChinese:
                    return stringT;
            }
            return "";
        }
    }
}

public enum GameStringType
{
    Title0 = 0,
    Title1,
    Title2,
    Title3,

    Shop0,
    Shop1,
    Shop2,
    
    Bag0,

    Camp0,
    Camp1,
    Camp2,
    Camp3,
    Camp4,
    Camp5,
    Camp6,

    CampExit0,
    CampExit1,

    ItemUI0,
    ItemUI1,
    ItemUI2,
    ItemUI3,

    AlchemyUI0,
    AlchemyUI1,

    PowerUI0,

    BattleInfoUI0,
    BattleInfoUI1,

    GameOver0,
    GameOver1,
    GameOver2,
    GameOver3,


    SL0,
    SL1,
    SL2,
    SL3,
    SL4,
    
    SaveProgress0,
    SaveProgress1,

    Time0,

    Agreement0,

    Help0,
    Help1,
    Help2,
    Help3,
    Help4,
    Help5,
    Help6,
    Help7,

    Help00,
    Help10,
    Help20,
    Help30,
    Help40,

    Setting0,
    Setting1,
    Setting2,
    Setting3,
    Setting4,
    Setting5,

    Setting01,
    Setting02,

    Setting11,
    Setting12,

    Setting21,
    Setting22,


    ActiveGame0,

    BattleInfoUI2,


    SaveTitle,
    LoadTitle,
}

public class GameStringData : Singleton<GameStringData>
{
    [SerializeField]
    List<GameString> data;

    public void load()
    {
        data = new List<GameString>();

        addString( "初章初始" );
        addString( "前历再续" );
        addString( "战阵重现" );
//        addString( "回返太虚" );
        addString( "设定帮助" );

        addString( "武器店" );
        addString( "饰品店" );
        addString( "杂货铺" );

        addString( "行囊" );

        addString( "囊内乾坤" );
        addString( "诸物炼化" );
        addString( "体态观览" );
        addString( "五魂化蕴" );
        addString( "今况记入" );
        addString( "前历再续" );
        addString( "空明流转" );

        addString( "确定要出发吗？" );
        addString( "确定要返回标题界面吗？" );

        addString( "给与" );
        addString( "装备" );
        addString( "卸下" );
        addString( "丢弃" );

        addString( "选择物品" );
        addString( "结果预测" );

        addString( "确定要保存调整？" );

        addString( "胜利条件" );
        addString( "失败条件" );

        addString( "是否重新开始本关卡游戏？" );
        addString( "是，继承等级金钱，但无法获得当前关卡熟练度。" );
        addString( "否，返回标题界面。" );
        addString( "感谢您购买本游戏。图片等素材归属原汉堂国际，代码版权归属狐兔网络，其他游戏正在重置中敬请期待。前几名玩家截图以下随机码发送至游戏群管理员可兑换奖励！" );

        addString( "熟练度：" );
        addString( "简单" );
        addString( "普通" );
        addString( "困难" );
        addString( "回合数：" );

        addString( "保存中......" );
        addString( "保存完成。" );

        addString( "游戏时间：" );

        addString( "存档保存在本机。" );


        addString( "" );
        addString( "" );
        addString( "" );
        addString( "" );
        addString( "" );
        addString( "" );
        addString( "帮助" );
        addString( "关闭" );

        addString( "" );
        addString( "" );
        addString( "" );
        addString( "" );
        addString( "" );

        addString( "难度设定" );
        addString( "语言设定" );
        addString( "修改器" );
        addString( "键盘大小" );
        addString( "" );
        addString( "" );

        addString( "简单模式" );
        addString( "困难模式" );

        addString( "简体" );
        addString( "繁體" );

        addString( "缩小" );
        addString( "放大" );

        addString( "点击解锁游戏！\n（未解锁无法存取进度）" );

        addString( "熟练度" );

        addString( "今况记入" );
        addString( "前历再续" );



    }

    void addString( string str )
    {
        GameString gs = new GameString();
        gs.init( str );

        data.Add( gs );
    }

    public string getString( GameStringType t )
    {
        string str = data[ (int)t ].String;

        return str;
    }
}
