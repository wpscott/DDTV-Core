
# DDTV-Core
DDTV-Core是用C# 和 .Net Core 3.1编写的AcFun/BiliBili直播播放工具。
该项目分为4个子项目。
**不提供32位程序。**

**AcFun弹幕相关已移至[AcFunDanmaku](https://github.com/wpscott/AcFunDanmaku)**

## AcFunLiveServer
AcFunLiveServer是一个简单的控制台项目用于获取AcFun的直播视频流。
### 使用方式
0. 添加[AcFunDanmu.nupkg](https://github.com/wpscott/AcFunDanmaku/releases/download/2020.06.12/AcFunDanmu.1.0.5.nupkg)
1. 编译
2. 运行AcFunLiveServer.exe或`AcFunLiveServer.exe 用户名 密码`。请确保62114端口没有被使用。
3. 使用任何你喜欢的播放器如VLC或MPV，访问http://localhost:62114/用户ID （如：69065、23682490或156843）
4. 观看直播

## BilibiliLiveServer
BiliBiliLiveServer是一个简单的控制台项目用于转发BiliBili的直播视频流。
### 使用方式
1. 编译
2. 运行BiliBIliLiveServer.exe。请确保29119端口没有被使用。
3. 使用任何你喜欢的播放器如VLC或MPV，访问http://localhost:29119/房间号 （如：12235923）
4. 观看直播
#### 例子
![例子](https://raw.githubusercontent.com/wpscott/DDTV-Core/master/sample/sample.png)
*GPU占用高是因为[Wallpaper Engine](https://www.wallpaperengine.io/)*

## DDTV-FFmpeg
基于WPF和[ffmediaelement](https://github.com/unosquare/ffmediaelement)的BiliBili直播播放器。（**该播放器占用较高**）
### 使用方式
1. 编译
2. 运行DDTV-FFmpeg.exe 房间号（如：12235923）
3. 观看直播

## DDTV-MPV
基于WinForms和[MPV.Net](https://github.com/hudec117/Mpv.NET-lib-)的BiliBili的直播播放器，使用AcFunLiveServer/BilibiliLiveServer作为直播源。
### 使用方式
1. 编译
2. 运行AcFunLiveServer.exe（请确保62114端口没有被使用）或BilibiliLiveServer.exe（请确保29119端口没有被使用）
3. 运行DDTV-MPV.exe 平台（a/ac/acfun/b/bili/bilibili） A站用户ID（如：69065、23682490或156843）/B站房间号（如：12235923）
例子：`DDTV-MPV.exe a 69065` `DDTV-MPV.exe b 12235923`
4. 观看直播
