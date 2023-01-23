
using ClinicServiceNamespace;
using Grpc.Net.Client;
using static ClinicServiceNamespace.ClinicService;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

using var channel = GrpcChannel.ForAddress("http://localhost:5001");
ClinicServiceClient client = new ClinicServiceClient(channel);

//var response = client.CreateClient(new CreateClientRequest()
//{
//    Document = "Document 123456",
//    Firstname = "Ivan",
//    Surname = "Gavrilov",
//    Patronymic = "Yurjevich"
//});

//if (response.ErrCode==0)
//{
//    Console.WriteLine($"Client #{response.ClientId} created.");
//}
//else
//{
//    Console.WriteLine($"Client create error\nError code: {response.ErrCode}\nError: {response.ErrMessage}");
//}

var allClietsResponse = client.GetAllClients(new());

if (allClietsResponse.ErrCode==0)
{
    Console.WriteLine("All Clinic Clients\n==========");
    foreach (var item in allClietsResponse.Clients)
    {
        Console.WriteLine($"Name: {item.Firstname} {item.Patronymic} {item.Surname}\nDoc: {item.Document}");
    }
}
else
{
    Console.WriteLine($"Request error\nError code: {allClietsResponse.ErrCode}\nError: {allClietsResponse.ErrMessage}");
}
Console.ReadKey();