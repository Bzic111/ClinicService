using ClinicService.Data;
using ClinicServiceNamespace;
using Grpc.Core;
using static ClinicServiceNamespace.ClinicService;

#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена

namespace ClinicServiceV2.Services
{
    public class ClinicService:ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _context;
        public ClinicService(ClinicServiceDbContext context)
        {
            _context = context;
        }

        public override Task<DeleteClientResponse> DeleteClient(DeleteClientRequest request, ServerCallContext context)
        {
            try
            {
                var client = _context.Clients.SingleOrDefault(c => c.Id == request.ClientId);
                if (client != null)
                {
                    _context.Clients.Remove(client);
                    _context.SaveChanges();
                    return Task.FromResult(new DeleteClientResponse { ErrCode = 1000, ErrMessage = "Seccessful" });
                }
                else
                {
                    return Task.FromResult(new DeleteClientResponse { ErrCode = 1004, ErrMessage = "No client with this Id" });
                }
            }
            catch (Exception e)
            {
                return Task.FromResult(new DeleteClientResponse { ErrCode = 1006, ErrMessage = "Internal server error 1006" + e.Message });
            }
        }
        public override Task<UpdateClientResponse> UpdateClient(UpdateClientRequest request, ServerCallContext context)
        {
            UpdateClientResponse response= new UpdateClientResponse();
            try
            {
                var client = _context.Clients.SingleOrDefault(c => c.Id == request.ClientId);
                if (client != null)
                {
                    client.Document = request.Document;
                    client.Firstname = request.Firstname;
                    client.Surname = request.Surname;
                    client.Patronymic = request.Patronymic;
                    _context.SaveChanges();
                    response.ErrCode = 1000;
                    response.ErrMessage = "Successful";
                    return Task.FromResult(response);
                }
                else
                {
                    return Task.FromResult(new UpdateClientResponse { ErrCode = 1004, ErrMessage = "No client with this Id" });
                }
            }
            catch (Exception e)
            {
                response.ErrCode = 1005;
                response.ErrMessage = "Internal server error 1005" + e.Message;
                return Task.FromResult(response);
            }
        }
        public override Task<GetClientByIdResponse> GetClientById(GetClientByIdRequest request, ServerCallContext context)
        {
            try
            {
                var client = _context.Clients.SingleOrDefault(c => c.Id == request.ClientId);
                if (client != null) 
                {
                    ClientResponse response = new ClientResponse
                    {
                        ClientId = client.Id,
                        Document = client.Document,
                        Surname = client.Surname,
                        Firstname = client.Firstname,
                        Patronymic = client.Patronymic
                    };
                    return Task.FromResult(new GetClientByIdResponse { Client = response, ErrCode = 0, ErrMessage = "" });
                }
                else
                {
                    return Task.FromResult(new GetClientByIdResponse { ErrCode = 1004, ErrMessage = "No client with this Id" });
                }
            }
            catch (Exception e)
            {
                return Task.FromResult(new GetClientByIdResponse { ErrCode = 1003, ErrMessage = "Internal server error 1003" + e.Message });
            }
        }
        public override Task<CreateClientResponse> CreateClient(CreateClientRequest request, ServerCallContext context)
        {
            try
            {
                var client = new Client
                {
                    Document = request.Document,
                    Surname = request.Surname,
                    Firstname = request.Firstname,
                    Patronymic = request.Patronymic
                };
                _context.Clients.Add(client);
                _context.SaveChanges();

                var response = new CreateClientResponse()
                {
                    ClientId = client.Id,
                    ErrCode = 0,
                    ErrMessage = "",
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                var response = new CreateClientResponse()
                {
                    ErrCode = 1001,
                    ErrMessage = "Internal server error 1001\n" + e.Message,
                };
                return Task.FromResult(response);
            }
        }
        public override Task<GetAllClientsResponse> GetAllClients(GetAllClientsRequest request, ServerCallContext context)
        {
            try
            {
                var clients = _context.Clients.Select(client => new ClientResponse()
                {
                    ClientId = client.Id,
                    Document = client.Document,
                    Firstname = client.Firstname,
                    Surname = client.Surname,
                    Patronymic = client.Patronymic
                }).ToList();
                var response = new GetAllClientsResponse();
                response.Clients.AddRange(clients);
                response.ErrCode = 0;
                response.ErrMessage = "";
                return Task.FromResult(response);

            }
            catch (Exception e)
            {
                var response = new GetAllClientsResponse()
                {
                    ErrCode = 1002,
                    ErrMessage = "Internal server error 1002\n" + e.Message,
                };
                return Task.FromResult(response);
            }

        }
    }
}
