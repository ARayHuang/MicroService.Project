## 建立新的服務
1. 微服務就是要放到Container上面，因此就拿netcore當作開發的語言(要先安裝)。
2. 可以參考netcore建置API的範例
```
mkdir netcoreWebAPI
cd netcoreWebAPI
dotnet new webapi
dotnet build
dotnet run 
```
這樣就可以完成一個最簡單的webAPI，用postman去打一下API應該就有東西了。

3. 但這樣只能算是一個執行程式，怎麼變成服務呢?就是要用docker把它佈署成為一個container。(Docker/Container的介紹，不會在這邊說)通常要將local的程式佈署到docker，會用Dockerfile將整包程式轉成image，再透過docker變成一個container讓他跑起裡面的程序(這樣就是一小顆服務了)。

以下開始介紹DockerFile
```
FROM mcr.microsoft.com/dotnet/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 4001

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["Base.Service.csproj", "./"]
RUN dotnet restore "Base.Service.csproj"
COPY . .
RUN dotnet build "Base.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Base.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Base.Service.dll"]
```
一個一個來說明
- FROM 就是要用哪一個image來執行這顆服務，也就是這顆服務的OS是什麼，裡面裝了那些軟體了(通常各大廠商都會開發好，讓開發人員使用)
- WORKDIR 表示移動到container裡面的工作目錄(類似cd)
- COPY 表示將local端的目錄(資料夾/檔案) 複製到container裡面的目錄
- RUN 表示執行某個動作(這邊就是執行dotnet ...)
- ENTRYPOINT 表示container啟動後，執行的命令
---
所以可以來說說上面一大塊做了哪些:
**前情提要:每個FROM產生的都是獨立的一個空間**
1. 一開始先用aspnet:3.1-focal的image當作runtime要使用的publish資料複製到base image空間，然後將目錄與port設定好，暫不使用。
2. 利用sdk:3.1-focal這個SDK來當作建置要使用的image空間，移動到src並將local端的程式複製到container裡面。然後執行restore跟build。
3. build沒有問題，用相同的方式打包成Release放置到/app/publish資料夾。 (2、3都是在build這個image空間)
4. 最後使用一開始的runtime base image空間來執行，先切換到app目錄，再將build空間的/app/publish資料複製到base空間的/app裡面，利用ENTRYPOINT執行。

看到這邊還沒暈的，真的很強!!!
再忍耐一下，快要結束了(好像才到一半)

---
這樣Dockerfile就完成了，把Dockerfile放到跟程式相同的目錄下就可以建置成docker image了(當然Dockerfile可以放在任意的目錄，只是要確認Dockerfile裡面的路徑寫法一定要對應到正確的目錄)

```cmd
--最後一定要有空白+點
docker build -t <image的名稱> . 
--看一下有沒有image
docker images
--沒問題可以執行這個image了
docker run -d --name=myapp --rm -p 8000:80 <image的名稱>
```
先說一下最後一行 docker run .....

可以執行docker的語法有幾百萬種(先建立之後再start, 執行background ...)一樣說一下參數。
- -d 再背景執行
- --name container名稱
- --rm 停止就刪除
- -p local的port對應到container的port 

大功告成，可以用postman去打localhost:8000看API有沒有正確
(記得要確認port跟API的route喔)



## API Gateway
API Gateway設什麼呢?主要是讓Client面對API只會有一個進入點。例如我有五顆服務，各自的port、route可能不是統一設計的規範，這樣管理起來相當麻煩。

可以透過API Gateway讓它重新route，讓client只要知道最後的API位置，其餘的都封裝在API Gateway。同時也可以在API Gateway處理log、JWT驗證、分流...等，每個request都需要處理的動作。

使用Ocelot作為API Gateway，跟上面微服務相同，建置一個new webapi的專案。在程式中補上一些設定即可完成。

1. 加入Ocelot.json
```
{
    "Routes": [    
        {
            "DownstreamPathTemplate": "/WeatherForecast/",
            "DownstreamScheme": "http",
            "DownstreamHostAndPorts": [
                {
                    "Host": "base.service",
                    "Port": 4001
                }
            ],
            "UpstreamPathTemplate": "/WF/",
            "UpstreamHttpMethod": [ "Get" ]
        },
        {
            "DownstreamPathTemplate": "/WeatherForecast/{id}",
            "DownstreamScheme": "http",
            "DownstreamHostAndPorts": [
                {
                    "Host": "base.service",
                    "Port": 4001
                }
            ],
            "UpstreamPathTemplate": "/WF/{id}",
            "UpstreamHttpMethod": [ "Get" ]
        },
        ...
    ]
}
```

2. 安裝Ocelot package，然後Program.cs使用Ocelot.json
```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((host, config) => {
            config.AddJsonFile("Ocelot.json"); //加入
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>()
            .UseUrls("http://*:5001");
        });
```

3. 在Startup.cs裡面調整
```
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddOcelot();  //加入
}
...

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    ...

    app.UseOcelot().Wait();//加入
}
```
這樣完，利用dotnet build and run就可以在本機測試是否有打到API Gateway的API(port記得看對應的是否正確)。

---
Dockerfile 就不在重複介紹，相同方式就可以佈署上docker
```
FROM mcr.microsoft.com/dotnet/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["API.Gateway.csproj", "./"]
RUN dotnet restore "API.Gateway.csproj"
COPY . .
RUN dotnet build "API.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.Gateway.dll"]
```

## Docker-compose
因為一個一個Dockerfile來建置container太花時間了。所以docker提供了可以一次佈署多個container的方式

在目錄執行
```
docker-compose up -d
```

docker-compose.yml
```
version: "3"

services:
  base.service:
    container_name: base.service
    hostname: base.service
    build:
      context: ./Base.Service
      dockerfile: Dockerfile
    image: base.service/latest
    ports:
      - "4001:4001"
    environment:
      "PUBLIC_PORT": "4001"
  cache.service:
    container_name: cache.service
    hostname: cache.service
    build:
      context: ./Cache.Service
      dockerfile: Dockerfile
    image: cache.service/latest
    ports:
      - "4002:4002"
    environment:
      "PUBLIC_PORT": "4002"
  API.Gateway:
    container_name: API.Gateway
    build:
      context: ./API.Gateway
      dockerfile: Dockerfile
    image: API.Gateway/latest
    ports:
      - "5001:5001"
    links:
      - base.service
      - cache.service
    depends_on:
      - base.service
      - cache.service
```

- 透過寫好的dockerfile來建置image
- 透過hostname的設置，讓container之間像是區域網路一樣可以用hostname來作溝通EX: base.service來 ping http//:cache.service 可以直接得到回應，這樣在APIGateway就可以直接設定host+port來跟其他服務溝通。

```
--查看所有container
docker ps -a 
--開始執行
docker start <containerID1> <containerID1> ... 
--停止
docker stop <containerID1> <containerID1> ...
刪除
docker rm <containerID1> <containerID1> ...
進入container
docker exec -it <containerID1> bash

```
以上，透過三個小服務(base.service,cache.service,api.gateway)，就可以跑起來一個小的微服務世界。真正的微服務還有很多可以架構的，待之後慢慢接觸...
