## 驗證與授權

### 驗證

驗證(Authentication)，表示確認用戶身份(User Identity)的動作。最常見的就是使用密碼，像金融公司的系統常常會有需要多因子(憑證、用戶代號...等)的驗證，主要就是用多重因子驗證，來確定使用者身份。

### 身份(剛好提一下)
身份(Identity)，表示一個人可以識別的資訊。
- Claim
    如一張身份證上，提供了姓名、身份證號碼、生日...等資料，這一個個key-value的資訊，就叫做一個Claim。
    ```
    public class Claim
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
    ```
- ClaimsIdentity
    由Claim所組成的一張身份識別集合，可以理解為身份證。裡面可以新增、刪除Claim，但要限制存取權限，不可以讓外部使用隨意更改。
    ```
    public class ClaimsIdentity
    {
        public ClaimsIdentity(IEnumerable<Claim> claims){}
        
        public virtual string Name { get; }

        public string Label { get; set; }
        
        public virtual string AuthenticationType { get; }
        
        public virtual void AddClaim(Claim claim);
        
        public virtual void RemoveClaim(Claim claim);
        
        public virtual void FindClaim(Claim claim);
    }
    ```
- ClaimsPrincipal 
    由ClaimsIdentity所組成的集合，可以理解為證件包。一個使用者可能有不同的身份證，來做不同資源存取。

### 授權
授權(Authorization)，表示用戶能夠存取資源的範圍。
- 限制存取的資源，需要通過驗證後才能抓取。
- 已通過驗證的用戶，帶著通過驗證的token來看是否能夠使用資源。
- 透過不同的授權規則，來限制各項資源的存取。授權政策(Authorization Policy)、驗證方案(Authentication Scheme)...等

### 小結一下
要處理系統登入/登出:
1. 先驗證該用戶，並取的用戶的身份。
2. 做授權，給予該用戶不同資源不同的存取權限。
3. 透過授權後的token，確認授權。前往使用API資源。

reference

https://www.cnblogs.com/savorboard/p/aspnetcore-identity.html


---

### JWT(Json Web Token)
是一種Json格式的開放標準，它定義了一種簡潔(compact)且自包含(self-contained)的方式，用於在雙方之間安全地將訊息作為 JSON 物件傳輸。而這個訊息是經過數位簽章(Digital Signature)，因此可以被驗證及信任。

- 授權(Authorization)：這是很常見 JWT 的使用方式，例如使用者從 Client 端登入後，該使用者再次對 Server 端發送請求的時候，會夾帶著 JWT，允許使用者存取該 token 有權限的資源。單一登錄(Single Sign On)是當今廣泛使用 JWT 的功能之一，因為它的成本較小並且可以在不同的網域(domain)中輕鬆使用。
- 訊息交換(Information Exchange)：JWT 可以透過公鑰/私鑰來做簽章，讓我們可以知道是誰發送這個 JWT，此外，由於簽章是使用 header 和 payload 計算的，因此還可以驗證內容是否遭到篡改。

### JWT的組成
1. Header 是一種對自我的聲明，無論 JWT 是簽章的還是加密的，這些聲明都表示了其所使用的演算法，通常也會表示要如何解析 JWT 的其餘部分。
2. Payload 通常所有使用者有興趣的資訊都會被放在 payload，也就是上面介紹的Claims。
3. signature/encryption data。

以上是參考 https://5xruby.tw/posts/what-is-jwt 的說明，裡面寫得很清楚。

### 小結一下
JWT 是一種用來授權/訊息交換的開放格式，通常用於Server驗證完後，會將token回傳到client端，可以放在cookies裡面。等待下次要抓取API時，在request的header中會帶這個token來做身份授權。
JWT裡面包含各種Claims，Identity Server可以從中得知是否可以存取該API資源，達到身份授權的處理。

```
eyJhbGciOiJSUzI1NiIsImtpZCI6IjQ3RTBBNzEzQ0Q0NjM3MzVEQ0MyRjMwRDBFOUIxQzMwIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2Mjg2NzgxNTMsImV4cCI6MTYyODY4MTc1MywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL3Jlc291cmNlcyIsImNsaWVudF9pZCI6Im0ybS5jbGllbnQiLCJqdGkiOiJCNzFBREJBNEY3OUVFNjdFQzM3QzhDMEREQjcyQUMxOSIsImlhdCI6MTYyODY3ODE1Mywic2NvcGUiOlsic2NvcGUxIl19.KqWwzzEJojqZ5-UkoQdILVv4FfcXzNVOj50BrzYPGAtyc7zGXzwDcEGpY0JSbW1FKz7IF_95jtwNEzMSwqBVp2QyhQZIQIuOOIn9FsP5GiemkQxZ1rO3qvWx8JpbFw7vDX39FbS-Y1pyLQMYbMfRAjmO85EhBnzuYsOpB2niJOxVAWoUrzNPSDJ6Mg13weDFoDrFIAbLCQGnE5tEhShKf-d0M287oAJfqNQZoeLyLq8wie81uNcdyNe5NMoxFSznp6tknWQxFJT3eebNVzaBAX8nn13bZqcrLIHol3jYBHUWlDqmHg9J_87GcXGIEyCHjot__2PvNv08Nk_nNqL58Q
```
上面就是一個JWT，可以到https://jwt.io/ 去解碼。可以看到許多Claims

使用postman的話，就可以在Authorization部分選擇Bearer Token，加入Token後，試著去發送需要授權的API，request header就會自動加入授權的Token。

以上對於驗證與授權的前情提要，接下來就是本次的重點OAuth2.0 與 Open ID Connect

---

## Open ID Connect 與 OAuth2.0

這是今天的主角，如同上面提到的。登入就是要先驗證再授權，而目前大家熟知的google/FB/Line 都提供了授權的方式，讓我們使用不同的平台。其實就是用到這樣的技術。
- Open ID Connect作為驗證時，規範了一個認證的標準機制。
- OAuth2.0 作為授權的規範。

什麼情況下需要用到這個呢?
1. 最常使用到的就是目前在社群平台上的授權登入，我想要使用codeSandbox來線上寫寫Javascript，直接使用github登入且授權後，根本不用再去申請一個帳號。
2. 當我們在一家公司(銀行內部)，可能有一堆系統要使用，EX:人資平台、電子公文、線上教育訓練...等，甚至在公司內部的不同業務系統，大部分公司也都有提供SSO(舊版的透過ldap)。新的SSO方式，就可以使用一個驗證服務，利用token來得到各業務系統的API資源。
3. 如同這個Repo開設的目的，學習了解微服務的架構。各服務間透過API的方式溝通，這當中的驗證授權就可以透過這種方式來控管存取資源。舉一個例子，今天我們接了一個簡單的外包專案，包含前台顯示與後台管理，實作了MVC的架構給客戶，這樣兩邊就需要兩套的登入機制。假如今天又再接了新的專案，驗證機制又需要重寫，那不如使用第三方驗證授權，而我們自己開發這個"第三方驗證授權"的系統，這樣就可以專注在外包專案的流程上。

跟傳統驗證有什麼區別的?
傳統驗證，一個平台需要一套的登入機制，用戶在每個平台上也都需要一份登入的驗證帳密。在管理上，相對來說不方便。而且部分平台，在用戶的考量下，並不覺得是值得信賴的平台(例如一些大陸網站、迷片網站)。直接提供帳密給他們，是相當有風險的。在這時候，使用第三方授權就可以信賴多了。

又或者，今天我們有三個外包的專案，假如使用一般的登入機制，三套系統的登入機制可能不同，帳密的規格也不同，這樣要管理起來會相當麻煩，這時候SSO就會相當好用。

強烈建議有想要弄懂第三方認證授權技術的，看一下下面的影片
https://www.bilibili.com/video/BV16b411k7yM?p=1
裡面是是用.net core + IdentityServer4 的方式

---
## 實作

### **授權方式**
假如沒有去看上面的影片，也沒有去查一下OAuth2.0 授權方式的話，這段可以再看一下:
- Authorization Code(最常看到的常用於server side render的系統) 
    1. 用戶嘗試想要使用某個尚未登入的資源/平台。
    2. 此平台/瀏覽器/MVC...在這邊統稱Client端，觸發API後往ResourceAPI抓取資料。
    3. 此時Client會連到IdentityServer去判斷是否授權，因為沒有授權，IdentityServer會跳出它們預設的驗證登入畫面(就像google那樣)。
    4. 用戶輸入帳密後，將資訊傳送到IdentityServer(如:google)，驗證身份。
    5. 驗證成功，跳出是否授權頁面。IdentityServer 回傳Authorization Code。
    6. Client拿著Authorization Code去IdentityServer拿到Access Token。
    7. Access Token存在cookies上，之後每次請求Request都帶Token去抓API資料。
- Implicit(SPA網頁)
    主要使用在前後端分離的專案上，前端透過oidc-client-js函式庫，直接與IdentityServer作驗證，成功後IdentityServer會回傳Token，存放在Cookies後，像後端API抓取資源，流程類似Authorization Code，少了第六點。
- Resource Owner Password Credentials(直接使用帳密)
    用戶提供帳密給Client後，Client拿著這個資料去跟IdentityServer要Access Token，之後要去抓API再帶入這個Access Token。
    因此「必須是使用者高度信賴的應用程式」才適合使用，且唯有前兩種皆不可行時，才會考慮使用當前類型的流程。因此，適用的情境，可能像公司內部的系統 或者 是老舊的legacy system。
- Client Credentials
    這個情境下是沒有用戶端，也就是機器對機器 server對IdentityServer，因此透過機器(server)上定義ClientId與ClientSecret來完成驗證，在獲取Access Token對API抓取資料。
    例如: 兩顆小的微服務，排程系統要去RM Server抓資料，就需要透過Client Credentials的授權方式來操作。

reference : https://medium.com/%E9%BA%A5%E5%85%8B%E7%9A%84%E5%8D%8A%E8%B7%AF%E5%87%BA%E5%AE%B6%E7%AD%86%E8%A8%98/%E7%AD%86%E8%A8%98-%E8%AA%8D%E8%AD%98-oauth-2-0-%E4%B8%80%E6%AC%A1%E4%BA%86%E8%A7%A3%E5%90%84%E8%A7%92%E8%89%B2-%E5%90%84%E9%A1%9E%E5%9E%8B%E6%B5%81%E7%A8%8B%E7%9A%84%E5%B7%AE%E7%95%B0-c42da83a6015

### **.net core + IdentityServer4**

- IdentityServer4是.net core裡面，結合Open ID Connect 與 OAuth2.0的一個很棒的Package。裡面也提供了很多範例跟QuickStart的code。

首先，先安裝樣板檔，這樣之後可以直接開啟一個完整的範例專案。
```
dotnet new -i IdentityServer4.Templates

//可以看看有那些樣板，IdentityServer4相關的都是
dotnet new 
```

接著找一個目錄，新增一個專案出來。這邊使用InMemory，也就是一些設定的資料跟用戶資料，都是寫在config裡面。
沒有到DB去抓，這樣太複雜了，之後再來實作。
```
cd D:\Source
dotnet new is4inmem --name=IdentityForMem
```
完成後，到目錄去看基本上已經都建置好了，只需要調整一些就好。

### **實作最簡單的Client Credentials**
1. 打開Config.cs，下方有定義*m2m.client*這個Client
```
ClientId = "m2m.client",   //另一個應用程式的ID，授權時會需要
ClientName = "Client Credentials Client",   //名稱 不重要

AllowedGrantTypes = GrantTypes.ClientCredentials, //授權方式
ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) }, // 金鑰 授權用

AllowedScopes = { "scope1" }  //允許的API資源，做資源控管用
```
2. 開啟一個應用程式，不管開的是.net core /.net framework，都可以。這當中需要有一個擴充的Package *IdentityModel*，需要將它加入到專案。

3. 這邊有建立另一個web api專案:IdentityResourceAPI，開啟方式如下:
```
//先建立一個新的webapi
dotnet new webapi --name=XXXX

//這邊專案大致上都是.net core，因為沒有灌2019，所以我都用終端機執行
dotnet restore //還原專案的參考項目
dotnet build //就是建置專案
dotnet run //執行專案，可以看執行的log會說目前執行在哪個port 預設是5000、5001

```

在IdentityResourceAPI有設定要去IdentityServer驗證授權，因 此在IdentityResourceAPI的Startup.cs裡面有一段，就是所有Request近來都要去遠端驗證的意思。

```
services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options => {
    options.Authority = "http://localhost:5000/";
    options.RequireHttpsMetadata = false;
    options.Audience = "scope1";
});
```

4. 實際的操作可以參考IdentityServer.Project\IdentityConsole，裡面的執行意思為:

    - 先用一個httpClient找出IdentityServer，然後抓到稱為*發現文檔*的資訊。這個*發現文檔*包含了所有IdentityServer的資訊。
    - 接著抓取Token，要對應在IdentityServer上Client的訊息。
    ```
    var accecToken = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = discovery.TokenEndpoint, 
        ClientId = "m2m.client",
        ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A", //511536EF-F270-4058-80CA-1C89C192F69A
        Scope = "scope1"
    });
    ```
    - 接著可以試著Call API，抓回資料。

    - 小結: 先抓發現文檔確定IdentityServer活著、抓取Token、發送請求資源的Request，IdentityResourceAPI Server拿Token去IdentityServer驗證，沒問題就可以進去抓取API回傳。

### **實作最熟悉的Resource Owner Password credentials**
1. 在IdentityServer的Client會有另一個new Client*wpf.client*，AllowedGrantTypes會是ResourceOwnerPassword。
2. 有一個IdentityWPF的應用程式專案，裡面有可以輸入帳密的欄位，點選click後，就會去跑驗證&授權。
3. 唯一的不同就是取Token時的method不同，裡面要帶入UserName/Password，其他都相同，就不多介紹了。

### **實作大魔王的Authorization Code**
1. 一樣在IdentityServerClient會新增一個interactive的Client，只是多了一些參數。
```
RedirectUris = { "http://localhost:5002/signin-oidc" }, //驗證成功導回網址
FrontChannelLogoutUri = "http://localhost:5002/signout-oidc", //Client登出導回網址
PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },退出登入導回網址

AllowOfflineAccess = true, 是否提供離線存取
AllowedScopes = { "openid", "profile", "scope2" },
RequirePkce = false, //Pkce是OAuth 2.0 協議裡的其中一種，預設打開，會出錯
AllowedCorsOrigins = new [] //處理跨域問題
{
    "http://localhost:3003",
}
```
2. IdentityServerClient的StartUp.cs有加入一些處理跨域請求問題的設定。
3. 有一個IdentityMVC的專案，是來模擬一個網站平台一進入Home時，沒有授權。因此需要去IdentityServer驗證授權後，才能出現畫面。如何建立這個MVC專案呢?
    - 開啟VS -> 新增專案 -> Web -> ASP .Net Core Web ->Web應用程式(模型-檢視-控制器)，HTTPS可以不要勾，驗證選無。就可以了
4. 在IdentityMVC地startUp.cs中加入驗證的方式。
```
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.SignInScheme = "Cookies";
    options.Authority = "http://localhost:5000";
    options.RequireHttpsMetadata = false;
    options.ClientId = "interactive";
    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
    options.SaveTokens = true;
    options.ResponseType = "code";
    options.Prompt = "consent";  //沒有導到是否授權頁，可以加入這個參數

    options.Scope.Clear();
    //options.Scope.Add("api1");
    options.Scope.Add("scope2");
    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
    options.Scope.Add(OidcConstants.StandardScopes.Profile);       
});
```
5. 整個執行流程就是
    - 開啟IdentityForMem專案(port:5000)
    - 開啟IdentityMVC專案(port:5002)
    - 網頁(可以開無痕)連到http://localhost:5002，應該馬上導轉到http://localhost:5000/connect/authorize?cl......後面還有一堆 的頁面，會出現可以輸入帳密的地方。
    喔忘記說 User資訊放在IdentityForMem\Quickstart\TestUsers.cs
    - 輸入帳密(alice/alice)，驗證成功後會到允許授權的頁面。
    - 點選允許後，導回http://localhost:5002/home 可以看到授權的API

6. 遇到的雷
    - invalid_scope，表示在client上設置的scope跟授權完的scope是有衝突的。例如:在IdentityServer中設定只能看scope1，但在client端的startup中卻要看scope2這樣就有問題。一定要設定apiResources 裡面包含scop。
    - invalid_RedirectUris，表示導回頁面的網址是有誤的，要檢查一下port對不對。
    - invalid_request code challenge required 這個是因為新的版本usePkce自動為true，這樣就需要定義challenge。在new Client中，加入RequirePkce = false,可以解決。
    - 出現登入頁，輸入完密碼一直停留在同一頁出現*User is not authenticated* - cookies問題，在MVC專案startup.cs加入
    ```
    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Lax
    });
    ```
    - 登入後，沒有出現授權頁 - 加入options.Prompt = "consent"
---

終於，紀錄完了。很多新的東西，說難也不會很難，別人都幫忙把範例寫好了，照抄應該就可以。難的是要把它變成自己的東西。

這就是下一部分了，要把上一章的微服務改成這種方式來驗證API，
1. 切出一個Identity.Service作為驗證
2. 結合DB做登入
3. 建立一個簡單的react登入頁面，嘗試前後端分離的驗證授權方式

把範例轉成自己的東西，以後要登入驗證授權的Server記得來看看這兩篇喔!!


---

## 完整執行順序

```
cd ..\IdentityServer.Project\IdentityForMem //目錄自行確認一下
dotnet build
dotnet run  

//port 5000
```
切換另一個terminal
```
cd ..\IdentityServer.Project\IdentityResourceAPI //目錄自行確認一下
dotnet build
dotnet run 

//port 5001
```

### Client Credentials
用VS2017開啟IdentityConsole專案，就可以下中斷點看token、response等資訊。

### Resource Owner Password credentials
用VS2017開啟IdentityWPF專案，執行後跳出視窗，輸入帳密alice/alice，就可以下中斷點看token、response等資訊。


### Authorization Code
假如有VS2019可以直接開啟IdentityMVC，然後建置。
假如沒有，開啟另一個Terminal
```
cd ..\IdentityServer.Project\IdentityMVC\IdentityMVC // 有兩層喔!!!要到csproj那一層
dotnet run 

//port 5002
```
用無痕開啟http://localhost:5002，就可以測試了

