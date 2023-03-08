# MassTransitTaskLeak
When using SendTo to new queue names, the number of tasks grows pretty quickly

# How to test it
Start MassTransitTaskLeak.Server inside the debugger, then start MassTransitTaskLeak.Client without debugging information

If Helper.ChangeQueueEveryRequest in MassTransitTaskLeak.Common project is set to true, the number of tasks in MassTransitTaskLeak.Server will quickly grow to over 1000 and the number of exchanges in RabbitMQ management will also grow
If it's set to false, the number of tasks in MassTransitTaskLeak.Server will stay low (around 100 on my machine)
