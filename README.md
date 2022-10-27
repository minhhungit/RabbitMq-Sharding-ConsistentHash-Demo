# Consistent hashing
Refer: https://github.com/rabbitmq/rabbitmq-server/tree/main/deps/rabbitmq_consistent_hash_exchange

Install plugin:
```
rabbitmq-plugins enable rabbitmq_consistent_hash_exchange
```

### Some note:
Not really good if we need order guarantee because:
- in case you add new queue to exchange, rabbitmq will assign message to difference queue compare with before adding (need to be careful when deploy, with "sharding" mode it's safer because number of queue is controlled by `set_policy` as I know at this time)
- or if your node is restarted, it also can send message to difference queue (because `consistent hashing ring is stored in memory` )

Can demo the 2nd case by adding 2 queues A, B to exchange.
Then publish 10 messages, queue A hold messages of company ID 1,2,4,5,9, queue B hold company 3,6,7,8.
Restart rabbitmq
Publish more new 10 messages, then company can be assigned to difference queue 

=> Quite dangerous, consider to use "sharding" if need order guarantee

### How to run consumer

```
$ .\ConsistentHashingConsumer 1
$ .\ConsistentHashingConsumer 2
$ .\ConsistentHashingConsumer 3
$ .\ConsistentHashingConsumer 4
```


# Sharding
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