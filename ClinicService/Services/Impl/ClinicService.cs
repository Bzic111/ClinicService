using ClinicService.Data;
using ClinicServiceNamespace;
using Grpc.Core;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicService.Services.Impl
{
    public class ClinicService : ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _context;
        public ClinicService(ClinicServiceDbContext context) 
        {
            _context = context;
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
                    ErrMessage = "Internal server error 1001\n" +e.Message,
                };
                return Task.FromResult(response);
            }
        }
        public override Task<GetAllClientsResponse> GetAllClients(GetAllClientsRequest request, ServerCallContext context)
        {
            try
            {
                var clients = _context.Clients.Select(client=>new ClientResponse()
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
                    ErrMessage = "Internal server error 1002\n"+e.Message,
                };
                return Task.FromResult(response);
            }
            
        }
    }
}
