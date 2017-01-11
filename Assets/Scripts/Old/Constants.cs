using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 全局常量
/// </summary>
public static class Constants{
    public const string VERSION = "V3";//V0（V4为当前API最新版本，V0为调试用API）
    public const string URL = "http://115.29.249.196/bespoke/ol/";
    public const string URL2 = "http://115.29.249.196/bespoke/IPAD_API/V1/";//潮流资讯
    public const string WSCURL = "http://wemall.bi-lang.com/api2.0/";
    public const string OrdersysURL = "http://test.bi-lang.com/ordersys/";//"http://orderapi.bi-lang.com/ordersys/";
    public const string OrdersysURL2 = "http://pro.bi-lang.com/";
    public const string ImageURL = "http://orderapi.bi-lang.com/ordersys/upload/goods/";//图片地址
    public const string CouponImageURL = "http://wemall.bi-lang.com/";
    public const string KuanShiCanKao = "http://orderapi.bi-lang.com/ordersys/upload/productwish/";//款式参考

    public const string YangShiTuPian = "http://pro.bi-lang.com/download/style/";//样式图片
    public const string MianLiaoTuPian = "http://pro.bi-lang.com/download/material/fabric/";//面料图片
    public const string NiuKouTuPian = "http://pro.bi-lang.com/download/material/button/";//钮扣图片
    public const string ThreadImage = "http://pro.bi-lang.com/download/material/thread/";//衣缝线图片
    public const string ShouYeGongGao = "http://pro.bi-lang.com/download/notice/";//首页公告
    public const string ChaoLiuZiXun = "http://115.29.249.196/bespoke/download/trend_information/";//潮流资讯    
    public const string SizeImage = "http://orderapi.bi-lang.com/ordersys/upload/sizepic/";//尺寸图片
    public const string MeasureImg = "http://test.bi-lang.com/bespoke/download/guide/";//尺寸测量图片

    public static Dictionary<string, Texture2D> ImagesDic = new Dictionary<string, Texture2D>();//图片字典

    public static string UI = "2D UI";
    public static string DefaultImage = "http://orderapi.bi-lang.com/ordersys/upload/user/pinus.png";

    public const string Token = "token";
    public const string UserName = "userName";
    public const string PassWord = "PassWord";
    public static bool IsRecommend = true;//TODO low b
    public static bool HomeShow = true;
    public static bool HomeBtnsShow = true;
    public static bool IsShopCar = false;
}
