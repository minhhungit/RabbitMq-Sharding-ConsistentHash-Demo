Refer: https://github.com/rabbitmq/rabbitmq-server/tree/main/deps/rabbitmq_sharding

Install plugin:
```
rabbitmq-plugins enable rabbitmq_sharding
```

Then config policy
```
.\rabbitmqctl set_policy images-shard "^shard.images$" '{\"shards-per-node\": 4, \"routing-key\": \"r1234\"}'
```

This will auto create 4 queue for us (on a node), we don't need to create queue by manual
Routing key is used to bind queues to exchange, can be any text


There are 2 ways to comsume 'shard' queue, either using "exchange name"
```
channel.BasicConsume("shard.images", false, consumer); // this way rabbitmq will auto assign consumer to queue
```

Note: using exchange name, rabbitmq will auto assign consumer to queue, so becareful don't run too much consumer (1 queue should have only 1 consumer)


Or specializing explicit queue name (as parameter), I highly recommend this way:

```
channel.BasicConsume(key, false, consumer); // .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 0"
```

=> We can control number of consumer easier (create windows service for each consumer for example, and fixed queue name as parameter). 
This way we can avoid multiple consumers on single queue => order guarantee


### How to run consumer

```
$ .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 0"
$ .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 1"
$ .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 2"
$ .\ShardConsumer.exe "sharding: shard.images - rabbit@JINPC - 3"
```