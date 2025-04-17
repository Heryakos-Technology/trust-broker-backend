using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System;
using broker.Data;
using broker.Models;
using AutoMapper;
using broker.Dto;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;
namespace Controllers
{   
    //   [Authorize]
    [Route("api/deals")]
    [ApiController]
    public class DealsController : ControllerBase
    {
        private readonly IRepository<Deals> _dealsRepository;
          private readonly IRepository<Broker> _brokerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;
        public DealsController(IRepository<Deals> repo, IRepository<Broker> brokerRepo,
            IRepository<Customer> customerRepo, IMapper mapper)
        {
            _dealsRepository = repo;
            _brokerRepository = brokerRepo;
            _customerRepository = customerRepo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetDeals()
        {
            var model = await _dealsRepository.GetData();
            return Ok(_mapper.Map<IEnumerable<DealsDto>>(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDealsById(int id)
        {
            Console.WriteLine("Returning Deals of id" + id);
            var model = await _dealsRepository.GetDataById(id);
            return Ok(_mapper.Map<DealsDto>(model));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateDeals(DealsDto dealsDto)
        {
            Console.WriteLine("Creating deals");
            var deals = _mapper.Map<Deals>(dealsDto);
            await _dealsRepository.UpdateData(deals);
            return Ok(dealsDto);
        }
        // [Authorize(Roles = RoleEntity.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeals(int id)
        {
            var model = await _dealsRepository.GetDataById(id);
            var deals = _mapper.Map<Deals>(model);
            await _dealsRepository.DeleteData(deals);
            return Ok(model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeals(int id, DealsDto  dealsDto)
        {
            // Console.WriteLine(technician.AccepteStatus);
            var deals = _mapper.Map<Deals>(dealsDto);
            await _dealsRepository.UpdateData(deals);
            return Ok(deals);
        }
        [HttpGet("customer")]
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Customer")]
         public async Task<IActionResult> GetCustomerDeals()
        {
            try
            {
                // Get UserId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    Console.WriteLine("Invalid or missing UserId claim in token.");
                    return Unauthorized("Invalid token.");
                }

                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                Console.WriteLine($"Token UserId: {userId}, Role: {roleClaim}");
                // Find the Customer by UserId
                var customer = await _customerRepository.GetDataById(userId); // Using GetByUserId
                if (customer == null)
                    return NotFound("Customer profile not found for this user.");

                // Get deals for the Customer
                var deals = await _dealsRepository.GetData();
                var customerDeals = deals.Where(d => d.CustomerId == customer.CustomerId);

                if (!customerDeals.Any())
                    return NotFound("No deals found for this customer.");

                return Ok(_mapper.Map<IEnumerable<DealsDto>>(customerDeals));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customer deals: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving customer deals.");
            }
        }

        [HttpGet("broker")]
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Broker")]
        // [Authorize(Roles = "Broker")]
            public async Task<IActionResult> GetBrokerDeals()
        {
            try
            {
                // Get UserId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid token.");

                // Find the Broker by UserId
                var broker = await _brokerRepository.GetDataById(userId); // Using GetByUserId
                if (broker == null)
                    return NotFound("Broker profile not found for this user.");

                // Get deals for the Broker
                var deals = await _dealsRepository.GetData();
                var brokerDeals = deals.Where(d => d.BrokerId == broker.BrokerId);

                if (!brokerDeals.Any())
                    return NotFound("No deals found for this broker.");

                return Ok(_mapper.Map<IEnumerable<DealsDto>>(brokerDeals));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving broker deals: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving broker deals.");
            }
        }
        //   public async Task<IActionResult> GetPaginatedBrokers(int pageNumber,int pageSize, string orderBy,string search)
        // {   
        //     // ServiceRepository _service= new ServiceRepository();
        //      Console.WriteLine("These are the comming constriant");
        //      Console.WriteLine(pageNumber);
        //      Console.WriteLine(orderBy);
        //      Console.WriteLine(search);
       
        //    var model = await _catigoryRepository.GetPaginatedData(pageNumber,pageSize,orderBy,search);

        // var totalPage= await _catigoryRepository.GetTotalPage(pageSize,search);
        // BrokerData broker= new BrokerData(totalPage,model);
             
        //      return Ok(broker);
        // }
        
    }

}