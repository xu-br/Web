# Docker Desktop 安装指南（Windows）

> 适用于 Windows 10/11  
> 生成时间：2026-04-23

---

## 一、系统要求

### Windows 11 / Windows 10 专业版/企业版/教育版

- **操作系统**：Windows 10 64-bit（版本 1903 或更高）或 Windows 11
- **CPU**：支持虚拟化的 64 位处理器
- **内存**：至少 4GB RAM（推荐 8GB）
- **硬盘**：至少 10GB 可用空间
- **BIOS**：启用虚拟化（Intel VT-x 或 AMD-V）

### Windows 10 家庭版

- 需要启用 WSL 2（Windows Subsystem for Linux 2）
- 其他要求同上

---

## 二、安装步骤

### 步骤 1：检查系统版本

```powershell
# 打开 PowerShell，执行
winver
```

确认版本号：
- Windows 10：版本 1903（内部版本 18362）或更高
- Windows 11：所有版本

### 步骤 2：启用虚拟化

#### 检查虚拟化是否已启用

```powershell
# 打开任务管理器（Ctrl + Shift + Esc）
# 切换到"性能"选项卡 → CPU
# 查看"虚拟化"是否显示"已启用"
```

#### 如果未启用，需要在 BIOS 中开启

1. 重启电脑，进入 BIOS（通常按 F2、F10、Del 或 Esc）
2. 找到虚拟化选项：
   - Intel CPU：Intel VT-x 或 Intel Virtualization Technology
   - AMD CPU：AMD-V 或 SVM Mode
3. 设置为 **Enabled**
4. 保存并退出 BIOS

### 步骤 3：启用 WSL 2（Windows 10 家庭版必须）

#### 3.1 启用 WSL 功能

以**管理员身份**打开 PowerShell，执行：

```powershell
# 启用 WSL
dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart

# 启用虚拟机平台
dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart
```

#### 3.2 重启电脑

```powershell
Restart-Computer
```

#### 3.3 下载并安装 WSL 2 Linux 内核更新包

1. 下载地址：https://aka.ms/wsl2kernel
2. 双击安装 `wsl_update_x64.msi`

#### 3.4 设置 WSL 2 为默认版本

```powershell
wsl --set-default-version 2
```

### 步骤 4：下载 Docker Desktop

1. 访问官网：https://www.docker.com/products/docker-desktop/
2. 点击 **Download for Windows**
3. 下载 `Docker Desktop Installer.exe`（约 500MB）

### 步骤 5：安装 Docker Desktop

1. 双击 `Docker Desktop Installer.exe`
2. 勾选 **Use WSL 2 instead of Hyper-V**（推荐）
3. 点击 **OK** 开始安装
4. 安装完成后点击 **Close and restart**

### 步骤 6：启动 Docker Desktop

1. 从开始菜单启动 **Docker Desktop**
2. 首次启动需要接受服务条款
3. 等待 Docker Engine 启动（右下角图标变为绿色）

### 步骤 7：验证安装

打开 PowerShell 或 CMD，执行：

```bash
# 检查 Docker 版本
docker --version

# 检查 Docker Compose 版本
docker-compose --version

# 运行测试容器
docker run hello-world
```

预期输出：
```
Hello from Docker!
This message shows that your installation appears to be working correctly.
```

---

## 三、Docker Desktop 配置

### 1. 资源配置（重要）

1. 打开 Docker Desktop
2. 点击右上角 **设置图标** → **Resources**
3. 调整资源分配：
   - **CPUs**：至少 2 核（推荐 4 核）
   - **Memory**：至少 4GB（推荐 8GB）
   - **Disk image size**：至少 60GB
4. 点击 **Apply & Restart**

### 2. WSL 集成（如果使用 WSL 2）

1. 设置 → **Resources** → **WSL Integration**
2. 启用 **Enable integration with my default WSL distro**
3. 点击 **Apply & Restart**

### 3. 镜像加速（可选，国内推荐）

1. 设置 → **Docker Engine**
2. 添加镜像源：

```json
{
  "registry-mirrors": [
    "https://docker.mirrors.ustc.edu.cn",
    "https://hub-mirror.c.163.com"
  ]
}
```

3. 点击 **Apply & Restart**

---

## 四、启动 ELK Stack

### 1. 进入项目目录

```bash
cd C:\Users\nnm43\Desktop\wrrr\Web.Api
```

### 2. 启动 ELK

```bash
# 启动服务
docker-compose -f docker-compose.elk.yml up -d

# 查看启动状态
docker-compose -f docker-compose.elk.yml ps

# 查看日志
docker-compose -f docker-compose.elk.yml logs -f elasticsearch
```

### 3. 等待服务启动

- **Elasticsearch**：约 30 秒
- **Kibana**：约 1-2 分钟

### 4. 验证服务

```bash
# 检查 Elasticsearch
curl http://localhost:9200

# 访问 Kibana
# 浏览器打开：http://localhost:5601
```

### 5. 停止服务

```bash
# 停止服务
docker-compose -f docker-compose.elk.yml down

# 停止并删除数据
docker-compose -f docker-compose.elk.yml down -v
```

---

## 五、常见问题

### 问题 1：Docker Desktop 无法启动

**症状**：Docker Desktop 一直显示 "Starting..."

**解决方案**：
1. 检查虚拟化是否启用（任务管理器 → 性能 → CPU）
2. 重启 Docker Desktop
3. 重启电脑
4. 卸载并重新安装 Docker Desktop

### 问题 2：WSL 2 安装失败

**症状**：提示 "WSL 2 installation is incomplete"

**解决方案**：
1. 确认已安装 WSL 2 内核更新包：https://aka.ms/wsl2kernel
2. 以管理员身份运行 PowerShell：
   ```powershell
   wsl --update
   wsl --set-default-version 2
   ```

### 问题 3：Elasticsearch 容器无法启动

**症状**：`docker-compose ps` 显示 elasticsearch 状态为 `Restarting`

**解决方案**：
1. 查看错误日志：
   ```bash
   docker logs elasticsearch
   ```
2. 常见原因：内存不足
   - 增加 Docker Desktop 内存限制（Settings → Resources → Memory）
   - 至少分配 4GB 内存

### 问题 4：端口被占用

**症状**：启动时提示 "port is already allocated"

**解决方案**：
1. 检查端口占用：
   ```powershell
   netstat -ano | findstr :9200
   netstat -ano | findstr :5601
   ```
2. 停止占用端口的进程或修改 `docker-compose.elk.yml` 中的端口映射

### 问题 5：Docker Desktop 占用资源过高

**解决方案**：
1. 降低资源分配（Settings → Resources）
2. 不使用时关闭 Docker Desktop
3. 配置 Docker Desktop 开机不自动启动（Settings → General → 取消勾选 "Start Docker Desktop when you log in"）

---

## 六、Docker 基础命令

### 容器管理

```bash
# 查看运行中的容器
docker ps

# 查看所有容器（包括停止的）
docker ps -a

# 停止容器
docker stop <container_id>

# 启动容器
docker start <container_id>

# 删除容器
docker rm <container_id>

# 查看容器日志
docker logs <container_id>

# 进入容器
docker exec -it <container_id> bash
```

### 镜像管理

```bash
# 查看本地镜像
docker images

# 删除镜像
docker rmi <image_id>

# 拉取镜像
docker pull <image_name>

# 清理未使用的镜像
docker image prune -a
```

### Docker Compose

```bash
# 启动服务
docker-compose up -d

# 停止服务
docker-compose down

# 查看服务状态
docker-compose ps

# 查看服务日志
docker-compose logs -f

# 重启服务
docker-compose restart
```

---

## 七、下一步

安装完成后，按照以下步骤测试 ELK：

1. **启动 ELK Stack**：
   ```bash
   docker-compose -f docker-compose.elk.yml up -d
   ```

2. **启动应用程序**：
   ```bash
   cd Web.Api
   dotnet run
   ```

3. **访问 Kibana**：
   - 浏览器打开：http://localhost:5601
   - 创建数据视图（参考 `ELK日志系统集成指南.md`）

4. **测试日志**：
   ```bash
   # 注册用户
   curl -X POST http://localhost:5000/api/Auth/register \
     -H "Content-Type: application/json" \
     -d '{"username":"test","displayName":"测试","email":"test@example.com","password":"123456"}'
   ```

5. **在 Kibana 中查看日志**：
   - 进入 Discover 页面
   - 搜索：`message: "用户注册"`

---

## 八、参考资料

- Docker 官方文档：https://docs.docker.com/desktop/install/windows-install/
- WSL 2 安装指南：https://docs.microsoft.com/zh-cn/windows/wsl/install
- Docker Compose 文档：https://docs.docker.com/compose/
- Elasticsearch 文档：https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html
- Kibana 文档：https://www.elastic.co/guide/en/kibana/current/index.html

---

**祝你安装顺利！如有问题，随时询问。**
