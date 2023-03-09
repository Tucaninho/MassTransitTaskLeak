# MassTransitTaskLeak
When using SendTo to new queue names, the number of in process Tasks grows pretty quickly

## How to test it
If Helper.ChangeQueueEveryRequest in MassTransitTaskLeak.Common project is set to true, the number of tasks in MassTransitTaskLeak.Server will quickly grow to over 1000
If it's set to false, the number of tasks in MassTransitTaskLeak.Server will stay low (around 100 on my machine)

In order to reproduce the issue using Visual Studio, follow those steps:

1. Set the Helper.ChangeQueueEveryRequest to the intended value
2. Start MassTransitTaskLeak.Server with debugging, and MassTransitTaskLeak.Client without debugging

    ![Solution Property](/Img/Start.png)

3. Start the test, once server and client are both connected, press a key on the client to start the tests
4. Let the test run for few seconds
5. Put a breakpoint in MassTransitTaskLeak.Common.Consumers.ConsumerRequest

    ![ConsumerRequest](/Img/Consumer.png)

6. Open the Task window (Debug -> Windows -> Tasks) as shown in the image before
7. Copy all the Tasks (CTRL+A - CTRL+C) and copy them in a tool as Excel or Notepad++ to count the number

    ![Tasks](/Img/Tasks.png)

7. Remove the breakpoint, and repeat steps 4 to 7 to see that the number of Tasks increases if Helper.ChangeQueueEveryRequest is true