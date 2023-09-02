# RabbitMQ-ConsoleApp-WithExchange
Çeşitli Exchange Tipleri kullanarak geliştirilen Console Publisher/Subscriber uygulamaları.

## Kullanılan Branch 'lar
Fanout-Exchange
Direct-Exchange
Topic-Exchange
Header-Exchange

## Ek Konular
Exchange ve Queue yaratırken kullanılan "Durable" parametresi "true" olarak tanımlanırsa, yaratılan Exchange veya Queue 'ler, RabbitMQ restart edildiğinde dahi silinmez.
Gönderilen Message 'in, RabbitMQ restart edildiğinde silinmemesi için aşağıdaki parametre kullanılır.

```
var properties = channel.CreateBasicProperties();
properties.Persistent = true; //Gönderilen Mesaj RabbitMQ restart edildiğinde silinmez.
```



