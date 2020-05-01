# DDTV-Core
DDTV-Core是用C# 和 .Net Core 3.1编写的BiliBili直播播放工具。
该项目分为3个子项目

## BilibiliLiveServer
BiliBiliLiveServer是一个简单的控制台项目用于转发BiliBili的直播视频流。
### 使用方式
1. 编译
2. 运行BiliBIliLiveServer.exe。请确保29119端口没有被使用。
3. 使用任何你喜欢的播放器如VLC或MPV，访问http://localhost:29119/房间号（如：12235923）
4. 观看直播

## DDTV-FFmpeg
基于WPF和FFmpeg的BiliBili直播播放器。
### 使用方式
1. 编译
2. 运行DDTV-FFmpeg.exe 房间号（如：12235923）
3. 观看直播

## DDTV-MPV
基于WinForms和MPV.Net的BiliBili的直播播放器，使用BilibiliLiveServer作为直播源。
### 使用方式
1. 编译
2. 运行DDTV-MPV.exe 房间号（如：12235923）
3. 观看直播