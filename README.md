# 前言

這個專案目的是想練習一些微服務的side project。

起初，看到安德魯的文章，有一題關於面試的題目。

```
請問如何處理交易的金流，同時間可能多筆付款，可能讀取餘額。

一個系統一秒有幾萬筆資料，如何在運作超過一段時間後，統計每分鐘/每小時的流量...等等的分析資料，同時不會效能低落。
```

解法有幾種，自己也實作了(等待某次的commit再來說)。當下覺得這樣的交易API，需要加上快取服務，甚至將交易與帳務拆成兩個微服務來處理，效能可能更好。

那...那服務之間要怎麼串起來，可以使用MQ(RabbitMQ、Kafka)，用Event Driven Development，把Log、背後要統計的資料、快取...等的動作，放在其他服務串起來。

這樣可能包含多個API service，記得之前看過的文章，可能會需要API Gateway、Service Discovery...等基礎架構。這些要怎麼架起來啊......，之後是不是還可以寫個前端的畫面來操作....

# 一步一步來吧
[Chapter-1 微服務的建立](Chapter/Chapter-1.md)
[Chapter-2 驗證與授權  ](Chapter/Chapter-2.md)


# 之後還想繼續完成的紀錄一下
1. Service Discovery
2. MQ Kafka
3. 安德魯的面試提目，用快取服務
4. Code Gen
5. JWT Token 