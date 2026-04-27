# ELK 日志系统集成指南

> 生成时间：2026-04-23  
> 适用项目：Web.Api

---

## 一、ELK Stack 简介

**ELK** 是一套强大的日志管理和分析解决方案，由三个开源组件组成：

- **Elasticsearch**：分布式搜索引擎，存储和索引日志数据
- **Logstash**：数据收集和处理管道（本项目未使用，直接用 Serilog 写入）
- **Kibana**：可视化界面，用于搜索、分析和展示日志数据

---

## 二、快速启动 ELK

### 1. 前置条件

- 安装 Docker Desktop
- 确保 Docker 正在运行
- 至少 4GB 可用内存

### 2. 启动 ELK Stack

在项目根目录执行：

```bash
# 启动 Elasticsearch + Kibana
docker-compose -f docker-compose.elk.yml up -d

# 查看启动状态
docker-compose -f docker-compose.elk.yml ps

# 查看日志
docker-compose -f docker-compose.elk.yml logs -f
```

### 3. 验证服务

**Elasticsearch**（约 30 秒启动）：
```bash
curl http://localhost:9200
```

预期响应：
```json
{
  "name" : "elasticsearch",
  "cluster_name" : "docker-cluster",
  "version" : {
    "number" : "8.11.0"
  }
}
```

**Kibana**（约 1-2 分钟启动）：
- 访问：http://localhost:5601
- 首次访问会显示欢迎页面

### 4. 停止 ELK

```bash
# 停止服务
docker-compose -f docker-compose.elk.yml down

# 停止并删除数据卷（清空所有日志）
docker-compose -f docker-compose.elk.yml down -v
```

---

## 三、项目集成说明

### 已集成的功能

#### 1. Serilog 配置

**位置**：`Web.Api/Program.cs`

**日志输出目标**：
- **控制台**：实时查看日志
- **文件**：`logs/webapi-{Date}.log`（按天滚动）
- **Elasticsearch**：索引格式 `webapi-logs-{yyyy.MM.dd}`

**日志级别**：
- 应用程序：`Information`
- Microsoft 框架：`Warning`

#### 2. 结构化日志

**示例**：`Web.Application/Services/RBAC/AuthService.cs`

```csharp
// 登录成功
_logger.LogInformation("用户登录成功: UserId={UserId}, Username={Username}, PermissionCount={PermissionCount}",
    user.Id, user.Username, permissions.Count);

// 登录失败
_logger.LogWarning("用户登录失败: 用户不存在 Username={Username}", input.Username);

// 注册成功
_logger.LogInformation("用户注册成功: UserId={UserId}, Username={Username}", user.Id, user.Username);
```

#### 3. HTTP 请求日志

自动记录所有 HTTP 请求：
```
HTTP POST /api/Auth/login responded 200 in 125.3456 ms
```

包含字段：
- `RequestMethod`：请求方法
- `RequestPath`：请求路径
- `StatusCode`：响应状态码
- `Elapsed`：耗时（毫秒）
- `RequestHost`：请求主机
- `RemoteIP`：客户端 IP

---

## 四、Kibana 使用指南

### 1. 首次配置

#### 步骤 1：创建数据视图（Data View）

1. 访问 http://localhost:5601
2. 点击左侧菜单 **☰** → **Management** → **Stack Management**
3. 点击 **Kibana** → **Data Views**
4. 点击 **Create data view**
5. 填写配置：
   - **Name**: `Web API Logs`
   - **Index pattern**: `webapi-logs-*`
   - **Timestamp field**: `@timestamp`
6. 点击 **Save data view to Kibana**

#### 步骤 2：查看日志

1. 点击左侧菜单 **☰** → **Analytics** → **Discover**
2. 选择刚创建的 **Web API Logs** 数据视图
3. 调整时间范围（右上角）：**Last 15 minutes** 或 **Last 1 hour**

### 2. 常用搜索

#### 基础搜索

```
# 搜索特定用户
fields.Username: "admin"

# 搜索登录日志
message: "登录"

# 搜索错误日志
level: "Error"

# 搜索警告和错误
level: ("Warning" OR "Error")

# 搜索特定时间段
@timestamp >= "2026-04-23T00:00:00" AND @timestamp <= "2026-04-23T23:59:59"
```

#### 高级搜索（KQL）

```
# 搜索登录失败
message: "登录失败" AND level: "Warning"

# 搜索特定用户的所有操作
fields.UserId: 123456789

# 搜索慢请求（超过 1 秒）
fields.Elapsed > 1000

# 搜索特定 IP 的请求
fields.RemoteIP: "192.168.1.100"

# 排除健康检查请求
NOT fields.RequestPath: "/health"
```

### 3. 创建可视化图表

#### 示例 1：登录成功/失败统计

1. 点击 **☰** → **Analytics** → **Visualize Library**
2. 点击 **Create visualization**
3. 选择 **Lens**
4. 配置：
   - **数据视图**: Web API Logs
   - **时间范围**: Last 24 hours
   - **过滤器**: `message: "登录"`
   - **Y 轴**: Count
   - **分组**: `level`（Success/Warning）
5. 保存为 **登录统计**

#### 示例 2：API 响应时间趋势

1. 创建新的 Lens 可视化
2. 配置：
   - **过滤器**: `fields.RequestPath: *`
   - **Y 轴**: Average of `fields.Elapsed`
   - **X 轴**: `@timestamp`（按小时分组）
3. 保存为 **API 响应时间**

### 4. 创建仪表板（Dashboard）

1. 点击 **☰** → **Analytics** → **Dashboard**
2. 点击 **Create dashboard**
3. 点击 **Add from library**
4. 选择之前创建的可视化图表
5. 调整布局
6. 保存为 **Web API 监控**

---

## 五、日志最佳实践

### 1. 日志级别使用

| 级别 | 使用场景 | 示例 |
|------|---------|------|
| **Trace** | 详细的调试信息 | 方法进入/退出 |
| **Debug** | 调试信息 | 变量值、中间状态 |
| **Information** | 正常业务流程 | 用户登录、订单创建 |
| **Warning** | 可恢复的异常 | 登录失败、参数验证失败 |
| **Error** | 错误但不影响系统运行 | 数据库连接失败、第三方 API 调用失败 |
| **Critical** | 严重错误，系统无法继续 | 数据库不可用、配置文件缺失 |

### 2. 结构化日志模板

```csharp
// ✅ 推荐：使用结构化参数
_logger.LogInformation("用户 {Username} 创建了订单 {OrderId}", username, orderId);

// ❌ 不推荐：字符串拼接
_logger.LogInformation($"用户 {username} 创建了订单 {orderId}");
```

### 3. 敏感信息处理

```csharp
// ❌ 不要记录密码、Token 等敏感信息
_logger.LogInformation("用户登录: Password={Password}", input.Password);

// ✅ 只记录必要的业务信息
_logger.LogInformation("用户登录: Username={Username}", input.Username);
```

### 4. 异常日志

```csharp
try
{
    // 业务逻辑
}
catch (Exception ex)
{
    _logger.LogError(ex, "处理订单失败: OrderId={OrderId}", orderId);
    throw;
}
```

---

## 六、性能优化

### 1. 日志采样

对于高频日志，可以使用采样：

```csharp
// 仅记录 10% 的请求
if (Random.Shared.Next(100) < 10)
{
    _logger.LogInformation("API 调用: Path={Path}", path);
}
```

### 2. 异步日志

Serilog 默认使用异步写入，无需额外配置。

### 3. 索引生命周期管理（ILM）

在 Elasticsearch 中配置索引生命周期策略：

```json
PUT _ilm/policy/webapi-logs-policy
{
  "policy": {
    "phases": {
      "hot": {
        "actions": {
          "rollover": {
            "max_size": "50GB",
            "max_age": "7d"
          }
        }
      },
      "delete": {
        "min_age": "30d",
        "actions": {
          "delete": {}
        }
      }
    }
  }
}
```

---

## 七、故障排查

### 问题 1：Elasticsearch 无法启动

**症状**：`docker-compose ps` 显示 `elasticsearch` 状态为 `Restarting`

**解决方案**：
```bash
# 查看错误日志
docker logs elasticsearch

# 常见原因：内存不足
# 解决：增加 Docker Desktop 内存限制（Settings → Resources → Memory）
```

### 问题 2：Kibana 无法连接 Elasticsearch

**症状**：访问 http://localhost:5601 显示 "Kibana server is not ready yet"

**解决方案**：
```bash
# 等待 Elasticsearch 完全启动（约 30 秒）
curl http://localhost:9200/_cluster/health

# 检查 Kibana 日志
docker logs kibana
```

### 问题 3：日志未出现在 Kibana

**症状**：Discover 页面没有数据

**解决方案**：
1. 检查时间范围（右上角）是否正确
2. 确认应用程序正在运行并产生日志
3. 检查 Elasticsearch 索引：
   ```bash
   curl http://localhost:9200/_cat/indices?v
   ```
4. 检查应用程序日志文件 `logs/webapi-{Date}.log`

### 问题 4：Serilog 连接 Elasticsearch 失败

**症状**：应用程序启动时报错 "No connection could be made"

**解决方案**：
1. 确认 Elasticsearch 正在运行：`curl http://localhost:9200`
2. 检查 `Program.cs` 中的 Elasticsearch URL 是否正确
3. 临时禁用 Elasticsearch Sink 进行测试：
   ```csharp
   // 注释掉 Elasticsearch 配置
   // .WriteTo.Elasticsearch(...)
   ```

---

## 八、生产环境建议

### 1. 安全配置

```yaml
# docker-compose.elk.yml
elasticsearch:
  environment:
    - xpack.security.enabled=true
    - ELASTIC_PASSWORD=your_strong_password
```

### 2. 持久化存储

```yaml
volumes:
  elasticsearch-data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: /path/to/persistent/storage
```

### 3. 集群部署

生产环境建议使用 Elasticsearch 集群（3+ 节点）以保证高可用。

### 4. 监控告警

配置 Kibana Alerting 监控关键指标：
- 错误日志数量
- API 响应时间
- 登录失败次数

---

## 九、常用命令速查

```bash
# 启动 ELK
docker-compose -f docker-compose.elk.yml up -d

# 停止 ELK
docker-compose -f docker-compose.elk.yml down

# 查看日志
docker-compose -f docker-compose.elk.yml logs -f elasticsearch
docker-compose -f docker-compose.elk.yml logs -f kibana

# 重启服务
docker-compose -f docker-compose.elk.yml restart

# 查看 Elasticsearch 健康状态
curl http://localhost:9200/_cluster/health?pretty

# 查看所有索引
curl http://localhost:9200/_cat/indices?v

# 删除旧索引（释放空间）
curl -X DELETE http://localhost:9200/webapi-logs-2026.04.01

# 查看索引文档数量
curl http://localhost:9200/webapi-logs-*/_count?pretty
```

---

## 十、学习资源

- **Elasticsearch 官方文档**：https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html
- **Kibana 官方文档**：https://www.elastic.co/guide/en/kibana/current/index.html
- **Serilog 文档**：https://serilog.net/
- **Serilog.Sinks.Elasticsearch**：https://github.com/serilog-contrib/serilog-sinks-elasticsearch

---

## 附录：配置文件说明

### Program.cs 配置

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                          // 最低日志级别
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // 覆盖框架日志级别
    .Enrich.FromLogContext()                             // 从日志上下文添加属性
    .Enrich.WithMachineName()                            // 添加机器名
    .Enrich.WithThreadId()                               // 添加线程 ID
    .WriteTo.Console(...)                                // 输出到控制台
    .WriteTo.File(...)                                   // 输出到文件
    .WriteTo.Elasticsearch(...)                          // 输出到 Elasticsearch
    .CreateLogger();
```

### Elasticsearch Sink 配置

```csharp
new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
{
    AutoRegisterTemplate = true,                         // 自动注册索引模板
    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,  // ES 版本
    IndexFormat = "webapi-logs-{0:yyyy.MM.dd}",         // 索引命名格式
    NumberOfShards = 2,                                  // 分片数量
    NumberOfReplicas = 1                                 // 副本数量
}
```

---

**祝你使用愉快！如有问题，请查看故障排查章节或联系技术支持。**
