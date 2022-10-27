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
$ .\ConsistantHashingConsumer 1
$ .\ConsistantHashingConsumer 2
$ .\ConsistantHashingConsumer 3
$ .\ConsistantHashingConsumer 4
```