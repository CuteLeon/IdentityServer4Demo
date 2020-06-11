# [Identity Server 4](http://www.identityserver.com.cn/)

# Auth

Authorization: 授权

Authentication: 身份认证

​	可以通过**授权**实现**身份认证**。

## 为什么OAuth2.0 AccessToken不能解决认证的问题？

1. AccessToken不包含身份认证信息
2. AccessToken生命周期可能很长
3. AccessToken可能被其他客户端直接借用
4. AccessToken不是为客户端准备的，而是为被保护的资源

# 协议

## OAuth 2.0

​	OAuth 2.0 是一个委托协议，用于实现授权（能够做什么），可以让控制资源的用户允许某个应用(程序)**代表**用户来访问用户控制的资源。应用可以从资源的所有者那里获得**授权(Authorization)**和**AccessToken**，随后即可使用**AccessToken**来访问资源。**资源的所有者是用户，应用是被保护资源的消费者。**

​	客户端应用无法获得用户名真正的账号和密码。

### 流程

​	授权服务器和资源服务器可以是物理独立的。

1. 客户端向用户发起授权请求(Request)
2. 用户回应允许(Grant)给客户端
3. 客户端使用用户的回应(Grant)向授权服务器请求Token
4. 授权服务器回应Token给客户端
5. 客户端使用Token向资源服务器请求资源(Resource)
6. 资源服务器回应被保护的资源(Resource)给客户端

### 授权类型

#### Authorization Code *

​	使用授权服务器作为客户端和资源服务器之间的中介。

​	AccessToken 在后台直接由授权服务发送给客户端应用，不经过用户浏览器，适用于服务端的MVC应用。

#### Implicit

​	简化模式，AccessToken 直接发送给用户浏览器，适合浏览器内的应用。

#### Resource Owner Password Credentials

​	世界使用客户端的用户名和密码作为凭证来访问资源。需要要求用户和客户端高度信任。

​	一般此类型仅仅在交换AccessToken时使用一次。

#### Client Credentials

​	用于客户端访问没有用户负责的资源。

#### Device Code

#### Refresh Token

### 端点

​	授权服务器有两个端点：Authorization Endpoint和Token Endpoint。

#### Authorization Endpoint

​	授权端点，用于和用户进行身份认证和同意授权。

#### Token Endpoint

​	Token端点，用于客户端交换AccessToken。

### Scope

​	Scope代表用户在资源服务器的权限范围。

### AccessToken

​	用于访问被保护资源的凭据。

​	代表了用户向客户端办法的授权。

​	需要描述出Scope和有效期等。

### Refresh Token

​	可选的

​	用于获取AccessToken的凭据。

​	可以让客户端逐渐降低访问权限。

### 发生错误时

#### 错误字段

- error
- error_description
- error_uri
- state

#### 错误形式

Authorization Endpoint 会通过 URL 的 QueryString 返回以上错误数据；

Token Endpoint 会通过响应的 Body 返回 Json 格式的以上错误数据；

#### 错误类型

1. invalid_request (400)
2. invalid_client (401)
3. invalid_grant (400)
4. unauthorized_client (400)
5. unsupported_grant_type (400)
6. invalid_scope (400)

## OpenId Connect

​	OpenId Connect 基于 OAuth 2.0 实现，用于实现用户认证（是谁）。

​	使用 ID Token，包含用户的信息，配合 AccessToken 一起使用。

​	UserInfo端点，用于获取用户信息。

​	预设一组标识身份的Scope和Claims: profile、email、address、phone

### 流程

1. 客户端(Relying Party)发送请求到身份提供商(OpenID Provider)。
2. 身份认证提供商验证用户身份和获得用户委派的权限，并回应AccessToken。
3. 客户端使用AccessToken向UserInfo端点发送请求。
4. 身份提供商回应用户Claims。

### 流程

#### Authorization Code Flow

​	验证码流程

#### Implicit Flow

​	简化流程

#### Hybrid Flow

​	验证码流程和简化流程的混合流程

# QuickStart

## 安装IdentityServer4模板

​	在CMD执行以下命令以安装模板：

```
dotnet new -i IdentityServer4.Templates
```

## 创建项目

​	使用以下命令快速创建一个IdentityServer4项目：

```
dotnet new mvc --auth Individual --name IdentityProvider
```

​	或者使用下面的模板：

```
dotnet new is4admin --name IdentityProvider
dotnet new is4aspid --name IdentityProvider
dotnet new is4empty --name IdentityProvider
dotnet new is4ef --name IdentityProvider
dotnet new is4inmem --name IdentityProvider
dotnet new is4ui --name IdentityProvider
```

