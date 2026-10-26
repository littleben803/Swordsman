using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;


public class GameIOSPayData : SingletonManager< GameIOSPayData > 
{

	public void release()
	{
		inited = false;
		isload = false;
	}

    public void init()
    {
#if !UNITY_IPHONE || UNITY_EDITOR
        return;
#endif
        if (inited)
        {
            return;
        }

        inited = true;

        IAPClearList();

        IAPGetList();
    }

#if UNITY_IPHONE
	[ DllImport("__Internal") ]
	private static extern void iapBuyGoods( int n );

	[ DllImport("__Internal") ]
	private static extern void iapFinishBuyGoods();

	[ DllImport("__Internal") ]
	private static extern void iapClearList();

	[ DllImport("__Internal") ]
	private static extern void iapGetList();
#endif

	string strLast = "";

	public bool isload = false;
	public bool inited = false;

	public void IAPRequest( string s )
	{
		isload = true;

		if ( strLast.Length > 1 )
		{
#if UNITY_IPHONE
            GamePHP.instance.phpIOSPay( strLast , onPayOver);
#endif
   		}
	}
	public void IAPComplete( string s )
	{
		strLast = s;

		if ( !isload ) 
		{
			return;
		}
#if UNITY_IPHONE
        GamePHP.instance.phpIOSPay( strLast , onPayOver );
#endif
    }
    public void IAPFailed( string s )
	{
		strLast = "";
	}
    public void onPayOver( int n , byte[] data )
    {
        if ( n == 1 )
        {
            IAPFinishBuyGoods();
            GameActiveUI.instance.unShow();
        }
    }

    public void IAPBuyGoods( int ii )
	{
		strLast = ii.ToString();

		if ( !isload ) 
		{
			return;
		}

		if ( strLast.Length > 1 ) 
		{
			return;
		}

#if UNITY_IPHONE
		iapBuyGoods( ii );
#endif
		strLast = ii.ToString();
	}

	public void IAPClearList()
	{
#if UNITY_IPHONE
		iapClearList();
#endif
	}

	public void IAPGetList()
	{
#if UNITY_IPHONE
		iapGetList();
#endif
	}

	public void IAPFinishBuyGoods()
	{
		strLast = "";
	#if UNITY_IPHONE
		iapFinishBuyGoods();
	#endif
	}

}
