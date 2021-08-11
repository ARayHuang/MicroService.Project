# IdentityService.Project

## 驗證與授權

### 驗證

驗證(Authentication)，表示確認用戶身份(User Identity)的動作。最常見的就是使用密碼，像金融公司的系統常常會有需要多因子(憑證、用戶代號...等)的驗證，主要就是用多重因子驗證，來確定使用者身份。

### 身份(剛好提一下)
身份(Identity)，表示一個人可以識別的資訊。
- Claim
    如一張身份證上，提供了姓名、身份證號碼、生日...等資料，這一個一個key-value的資訊，在.net中就叫做一個Claim

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
- 限制存取的資源，需要通過驗證後抓取。
- 已通過驗證的用戶，帶著通過驗證的token來看是否能夠使用資源。
- 透過不同的授權規則，來限制各項資源的存取。授權政策(Authorizaton Policy)、驗證方案(Authencation Scheme)...等


### 小結一下
要處理系統登入/登出:
1. 先驗證該用戶，並取的用戶的身份。
2. 做授權，給予該用戶不同資源不同的存取權限。
3. 透過授權後的token，確認授權。前往使用API資源。

refenece

https://www.cnblogs.com/savorboard/p/aspnetcore-identity.html


---

### JWT(Json Web Token)
是一種Json格式的開放標準，它定義了一種簡潔(compact)且自包含(self-contained)的方式，用於在雙方之間安全地將訊息作為 JSON 物件傳輸。而這個訊息是經過數位簽章(Digital Signature)，因此可以被驗證及信任。

- 授權(Authorization)：這是很常見 JWT 的使用方式，例如使用者從 Client 端登入後，該使用者再次對 Server 端發送請求的時候，會夾帶著 JWT，允許使用者存取該 token 有權限的資源。單一登錄(Single Sign On)是當今廣泛使用 JWT 的功能之一，因為它的成本較小並且可以在不同的網域(domain)中輕鬆使用。
- 訊息交換(Information Exchange)：JWT 可以透過公鑰/私鑰來做簽章，讓我們可以知道是誰發送這個 JWT，此外，由於簽章是使用 header 和 payload 計算的，因此還可以驗證內容是否遭到篡改。

### JWT的組成
1. Header 是一種對自我的聲明，無論 JWT 是簽章的還是加密的，這些聲明都表示了其所使用的演算法，通常也會表示要如何解析 JWT 的其餘部分。
2. Payload 通常所有使用者有興趣的資訊都會被放在 payload 
3. signature/encryption data

以上是參考 https://5xruby.tw/posts/what-is-jwt 的說明，裡面寫得很清楚。

### 小結一下
JWT 是一種用來授權/訊息交換的開放格式，通常用於Server驗證完後，會將token回傳到client端，可以放在cookies裡面。等待下次要抓取API時，在request的header中會帶著這個token來做身份授權。
JWT裡面包含各種Claims，Identity Server可以從中得知是否可以存取該API資源，達到身份授權的處理。

---