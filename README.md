
# DDTV-Core
DDTV-Core是用C# 和 .Net Core 3.1编写的AcFun/BiliBili直播播放工具。
该项目分为5个子项目。
**不提供32位程序。**

## AcFunDanmu 
AcFun直播弹幕解析工具。**开发中**

在AcFunDanmu文件夹中运行`protoc -I .\protos --csharp_out=.\Models [文件名].proto`生成C# Protobuf文件。需安装[Google Protocol Buffers](https://github.com/protocolbuffers/protobuf/releases/tag/v3.12.2)。

测试数据来源于`m.acfun.cn.har`，可以在Chrome或Firefox中导入并查看websockets。

### 使用方式
1. 复制/添加/导入AcFunDanmu文件夹到你的解决方案
2. 添加AcFunDanmu的项目引用
3. 添加以下代码片段
```
Using AcFunDanmu;
...
var client = new Client(用户ID);
或
var client = new Cient(用户ID, ServiceToken, SecurityKey, Tickets, EnteryRoomAttach, LiveId);

client.Handler = (你的自定义函数);
await client.Start();
```
*具体请参考AcFunDanmuConsole*

## AcFunDanmConsole
使用AcFunDanmu的控制台AcFun直播弹幕输出工具。
### 使用方式
1. 编译
2. 运行`AcFunDanmuConsole.exe 用户ID（如：69065、23682490或156843）`
3. 查看弹幕

## AcFunLiveServer
AcFunLiveServer是一个简单的控制台项目用于获取AcFun的直播视频流。
### 使用方式
1. 编译
2. 运行ACFunLiveServer.exe。请确保62114端口没有被使用。
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
2. 运行ACFunLiveServer.exe（请确保62114端口没有被使用）或BilibiliLiveServer.exe（请确保29119端口没有被使用）
3. 运行DDTV-MPV.exe 平台（a/ac/acfun/b/bili/bilibili） A站用户ID（如：69065、23682490或156843）/B站房间号（如：12235923）
例子：`DDTV-MPV.exe a 69065` `DDTV-MPV.exe b 12235923`
4. 观看直播
