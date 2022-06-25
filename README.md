# MockSocket

MockSocket是一个免费、开源、专注于内网穿透的高性能的反向代理应用, 支持TCP、UDP 等多种协议。可以将内网服务以安全、便捷的方式通过具有公网IP节点的中转暴露到公网。(后期尝试p2p 打洞)

## 使用

Server

`.\MockSocket.Server.exe -p 9090`

Agent

`.\MockSocket.Agent.exe -p 8080 -rs localhost -rsp 80`

Client

`curl http://localhost:8080`

## 特点

对比主流的frp(go), ngrok(c), 功能没那么多, 追求轻、快

- 基于 .NET 6
- server, agent端自定义任意端口,支持私有化部署
- 高性能:异步I/0(理想情况无线程占用)、零拷贝、BufferPool
- 真开箱即用:无需注册,登录
- 真跨平台: PC(Windows, Linux, macOS), Mobile(iOS, Android)...
  - MAUI已集成测试中..
- 扩展性强(基于消息驱动)、效率高(每个新连接最多增加一个单向数据包)