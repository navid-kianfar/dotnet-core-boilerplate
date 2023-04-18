# MyApplication - Boilerplate

### MyApplication.Abstraction

### MyApplication.Business

### MyApplication.DataAccess

### MyApplication.Endpoint
#### BaseController
#### AccountController
#### StorageController

### MyApplication.Workers

#### QueueListenerWorker
This is a base queue listener that will listen on a queue and will trigger when an item is pushed to it
and **ExecuteAction** method will be called with the input type casted to the given input type
if the process is successful the item will automatically acked from the queue
if process fails item will be rejected and goes back to the queue
Note: in real scenarios here we have to add a fail retry mechanism or something but it depends
on the application business (you may need to process items in pushed order so you can not go to the next node)
#### AccountBackgroundWorker
This background worker is responsible to long running processes related to actions
affecting an account, there might be a need to send an verification email or an otp...

### MyApplication.Tests.Integration
#### DatabaseSeed
#### EndpointIntegrationTestBase

### MyApplication.Tests.Unit
 