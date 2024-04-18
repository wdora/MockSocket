# MockSocket

MockSocket是一个免费、开源、专注于内网穿透的高性能的反向代理应用, 支持TCP、UDP 等多种协议。可以将内网服务以安全、便捷的方式通过具有公网IP节点的中转暴露到公网。(后期尝试p2p 打洞)

## 特点

对比主流的frp(go), ngrok(c), 功能没那么多, 追求轻、快

- 基于 .NET 6
- server, agent端自定义任意端口,支持私有化部署
- 高性能：异步I/O(理想情况无线程占用)、零拷贝、BufferPool
- 真开箱即用：无需注册,登录
- 真跨平台: PC(Windows, Linux, macOS), Mobile(iOS, Android)...
  - MAUI已集成测试中..
- 扩展性强(基于消息驱动)、效率高(每个新连接最多增加一个单向数据包)

### 性能测试

- [端口转发 详细对比](./docs/ab.md)
  - MockSocket 端口转发模式：`3759.17 [#/sec] (mean)`
  - nginx tcp模式：`2876.93 [#/sec] (mean)`
  - nginx http模式：`2253.12 [#/sec] (mean)`

- [内网穿透 详细对比](./docs/ab.md)
  - MockSocket 内网穿透模式: `89.97 [#/sec] (mean)`
  - frp tcp模式：`76.98 [#/sec] (mean)`

## 使用

### Docker版

Server

`docker run -d --name mocksocket-server --network host wdora/mocksocket-server:0.0.1`

### Cli版(支持 Windows、Linux、Mac)

#### 内网穿透

Server

`.\MockSocket.Server.exe -p 9090`

Agent

`.\MockSocket.Agent.exe -p 8080 -rs localhost -rsp 80 -hs mocksocket.com -hsp 9090`

Client

`curl http://mocksocket.com:8080`

#### 端口转发

Agent(default:agent)

`.\MockSocket.Agent.exe -p 8080 -rs localhost -rsp 80 -t proxy`

## tips

如果希望对端口设限制，可通过 docker + iptables 配合：

`docker run wdora/mocksocket-server:0.0.1 -p 9090:9090`

启用iptables(8080-50000)端口转发：

`iptables -t nat -A DOCKER -p tcp --dport 8080:50000 -j DNAT --to-destination 172.17.0.7`(172.17.0.7 为 container IP)

清理iptables：

`iptables -t nat -D DOCKER -p tcp --dport 8080:50000 -j DNAT --to-destination 172.17.0.7`