# AcfunDanmu AcFun直播弹幕工具

Source: [mplayer.js](https://cdnfile.aixifan.com/static/@ks/mplayer.5d57772120f807160aed.js)

## AcFun直播websocket数据结构

| 起始位置，偏移量  |  结构 |  说明 |
|---|---|---|
|  0, 12 |  ABCD 0001 FFFF FFFF FFFF FFFF |  ABCD 0001为Magic Number， 第一组FFFF FFFF为头数据长度，第二组FFFF FFFF为具体数据长度 |
|  12, 头数据长度 | [PacketHeader.proto](https://github.com/wpscott/DDTV-Core/blob/master/AcFunDanmu/protos/PacketHeader.proto) |  具体数据结构请查看[PacketHeader.proto](https://github.com/wpscott/DDTV-Core/blob/master/AcFunDanmu/protos/PacketHeader.proto) |
|  12 + 头数据长度, 16 |  FFFF FFFF FFFF FFFF FFFF FFFF FFFF FFFF |  4组int32作为AES IV，加解密用 |
|  28 + 头数据长度, 具体数据长度 | AES加密的[UpstreamPayload.proto](https://github.com/wpscott/DDTV-Core/blob/master/AcFunDanmu/protos/UpstreamPayload.proto)或[DownstreamPayload.proto](https://github.com/wpscott/DDTV-Core/blob/master/AcFunDanmu/protos/DownstreamPayload.proto) | 密钥为SecurityKey或SessionKey（由[PacketHeader.proto](https://github.com/wpscott/DDTV-Core/blob/master/AcFunDanmu/protos/PacketHeader.proto)中的encryptionMode指定）。根据command选择对应的protobuf进行进一步的payload解析 |

## AcFun直播websocket流程

1. 建立websocket链接
2. 发送RegisterRequest
3. 接收RegisterResponse，获取SessionKey
4. 发送KeepAliveRequest
5. 发送zt.live.interactive.ZtLiveCsCmd，payload为ZtLiveEnterRoom
6. 接收zt.live.interactive.ZtLiveCsCmd，payload为ZtLiveEnterRoomAck
7. 发送/接收弹幕及礼物，具体请查看zt.live.interactive.proto
8. 接收zt.live.interactive.ZtLiveCsHeartbeat，发送zt.live.interactive.ZtLiveCsHeartbeatAck
9. 接收UnregisterResponse，直播结束/发送UnregisterRequest，退出直播
