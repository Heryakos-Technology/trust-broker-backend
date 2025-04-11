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
    [Route("api/delivery")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IRepository<Delivery> _deliveryRepository;
        private readonly IRepository<Broker> _brokerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public DeliveryController(IRepository<Delivery> repo,IRepository<Broker> brokerRepo,
            IRepository<Customer> customerRepo, IRepository<User> userRepo,IMapper mapper)
        {
            _deliveryRepository = repo;
            _brokerRepository = brokerRepo;
            _customerRepository = customerRepo;
            _mapper = mapper;
            _userRepository = userRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetDeliveries()
        {
            var model = await _deliveryRepository.GetData();
            return Ok(_mapper.Map<IEnumerable<DeliveryDto>>(model));
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetDeliveryById(string  email)
        {
            Console.WriteLine("Returning delivery  of id" + email);
            var model = await _deliveryRepository.GetByEmail(email);
            return Ok(_mapper.Map<DeliveryDto>(model));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateDelivery(DeliveryDto deliveryDto)
        {
            Console.WriteLine("Creating delivery");
            var delivery = _mapper.Map<Delivery>(deliveryDto);
            await _deliveryRepository.InsertData(delivery);
            return Ok(deliveryDto);
        }
        // [Authorize(Roles = RoleEntity.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var model = await _deliveryRepository.GetDataById(id);
            var delivery = _mapper.Map<Delivery>(model);
            await _deliveryRepository.DeleteData(delivery);
            return Ok(model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, DeliveryDto  deliveryDto)
        {
            // Console.WriteLine(technician.AccepteStatus);
            var delivery = _mapper.Map<Delivery>(deliveryDto);
            await _deliveryRepository.UpdateData(delivery);
            return Ok(delivery);
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


        [HttpGet("customer")]
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Customer")]
        // [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerDeliveries()
        {
            try
            {
                // Get UserId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid token.");

                // Find the Customer by UserId
                var customer = await _customerRepository.GetDataById(userId);
                if (customer == null)
                    return NotFound("Customer profile not found for this user.");
                
                // Get deliveries for the Customer
                var deliveries = await _deliveryRepository.GetData();
                var customerDeliveries = deliveries.Where(d => d.CustomerId == customer.CustomerId);
                
                if (!customerDeliveries.Any())
                    return NotFound("No deliveries found for this customer.");

                return Ok(_mapper.Map<IEnumerable<DeliveryDto>>(customerDeliveries));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customer deliveries: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving customer deliveries.");
            }
        }

        [HttpGet("broker")]
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles = "Broker")]

        // [Authorize(Roles = "Broker")]
        public async Task<IActionResult> GetBrokerDeliveries()
        {
            try
            {
                // Get UserId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine("The Claim: ", userIdClaim);

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid token.");

                // Find the Broker by UserId
                var broker = await _brokerRepository.GetDataById(userId);
                Console.WriteLine("The User id: ", userId);
                if (broker == null)
                    return NotFound("Broker profile not found for this user.");
                
                // Get deliveries for the Broker
                var deliveries = await _deliveryRepository.GetData();
                var brokerDeliveries = deliveries.Where(d => d.BrokerId == broker.BrokerId);
                if (!brokerDeliveries.Any())
                    return NotFound("No deliveries found for this broker.");

                return Ok(_mapper.Map<IEnumerable<DeliveryDto>>(brokerDeliveries));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving broker deliveries: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving broker deliveries.");
            }
        }
        
    }

}