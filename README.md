# sharp-dev-lib-extension

## 介绍

sharp-dev-lib-extension是c#中经常用到的服务。

## 依赖说明

sharp-dev-lib-extension使用netstandard2.1框架，依赖第三方库如下：

* **DocumentFormat.OpenXml (>= 2.16.0)**
    用于Excel的读写

* **Portable.BouncyCastle (>= 1.9.0)**
    用于加解密

* **SharpDevLib(>= 1.0.4)**
    内部语法调用

## 安装教程

1. 从nuget包管理器中搜索安装[SharpDevLib.Extensions](https://www.nuget.org/packages/SharpDevLib.Extensions)

2. 在Package Manager中执行命令
   
   ```
   Install-Package SharpDevLib.Extensions -Version 1.1.0
   ```

3. 在dotnet cli中执行命令

```
dotnet add package SharpDevLib.Extensions --version 1.1.0
```

sharp-dev-lib-extension包含如下服务

| 名称                                                                                                                                                | 描述                                    | 使用   |
| ------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------- | ---- |
| [Data](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Data/DataExtension.cs)                      | 包含仓储，sql，迁移服务                         | 依赖注入 |
| [DI](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/DI/DIExtension.cs)                            | 提供程序集级别的服务注册                          | 扩展方法 |
| [Email](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Email/EmailExtension.cs)                   | 发送邮件                                  | 依赖注入 |
| [Encryption](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Encryption/EncryptionExtension.cs)    | 当前支持的算法，对称加密（aes,des,3des），非对称加密（rsa） | 依赖注入 |
| [Excel](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Excel/ExcelExtension.cs)                   | excel读写，支持复杂及动态对象，文档加密                | 依赖注入 |
| [Model](https://gitee.com/developer333/sharp-dev-lib-extensions/tree/master/src/SharpDevLib.Extensions/Model)                                     | 基础模型（Request,DTO,Response）            | 实例化  |
| [Transport](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Transport/SocketExtension.cs)          | udp,tcp的封装                            | 依赖注入 |
| [Http](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Http/HttpExtension.cs)                      | HttpClient的封装                         | 依赖注入 |
| [Jwt](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Jwt/JwtExtension.cs)                         | jwt创建和验证服务                            | 依赖注入 |
| [Certificate](https://gitee.com/developer333/sharp-dev-lib-extensions/blob/master/src/SharpDevLib.Extensions/Certificate/CertificateExtension.cs) | 证书创建                                  | 依赖注入 |

## 版本说明

| 时间         | 版本             | 描述                  |
| ---------- | -------------- | ------------------- |
| 2024-04-09 | v1.1.0          | 更新packages               |
| 2023-07-04 | v1.0.0.5       | 修复一些bug，添加sqlhelper  |
| 2022-08-12 | v1.0.0.0-alpha | 初始化包                    |