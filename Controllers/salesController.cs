using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System;
using broker.Data;
using broker.Models;
using AutoMapper;
using broker.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Linq;

namespace Controllers
{   
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IRepository<Sales> _salesRepository;
        private readonly IBrokerRepository _brokerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public SalesController(IRepository<Sales> repo,   IBrokerRepository brokerRepo, 
            ICustomerRepository customerRepo, IMapper mapper)
        {
            _salesRepository = repo;
            _brokerRepository = brokerRepo;
            _customerRepository = customerRepo;
            _mapper = mapper;
        } 
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            var model = await _salesRepository.GetData();
            return Ok(_mapper.Map<IEnumerable<SalesDto>>(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesById(int id)
        {
            Console.WriteLine("Returning Sales of id" + id);
            var model = await _salesRepository.GetDataById(id);
            return Ok(_mapper.Map<SalesDto>(model));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreatSales(SalesDto salesDto)
        {
            Console.WriteLine("Creating Sales");
            var sales = _mapper.Map<Sales>(salesDto);
            await _salesRepository.UpdateData(sales);
            return Ok(salesDto);
        }
        // [Authorize(Roles = RoleEntity.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSales(int id)
        {
            var model = await _salesRepository.GetDataById(id);
            var sales = _mapper.Map<Sales>(model);
            await _salesRepository.DeleteData(sales);
            return Ok(model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSales(int id, SalesDto  salesDto)
        {
            // Console.WriteLine(technician.AccepteStatus);
            var sales = _mapper.Map<Sales>(salesDto);
            await _salesRepository.UpdateData(sales);
            return Ok(sales);
        }

        [HttpGet("customer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
        public async Task<IActionResult> GetCustomerSales()
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
                var customer = await _customerRepository.GetDataById(userId);
                if (customer == null)
                    return NotFound("Customer profile not found for this user.");

                // Get sales for the Customer
                var sales = await _salesRepository.GetData();
                var customerSales = sales.Where(s => s.CustomerId == customer.CustomerId);

                if (!customerSales.Any())
                    return NotFound("No sales found for this customer.");

                return Ok(_mapper.Map<IEnumerable<SalesDto>>(customerSales));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customer sales: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving customer sales.");
            }
        }

        [HttpGet("broker")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Broker")]
        public async Task<IActionResult> GetBrokerSales()
        {
            try
            {
                // Get UserId from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized("Invalid token.");

                // Find the Broker by UserId
                var broker = await _brokerRepository.GetDataById(userId);
                if (broker == null)
                    return NotFound("Broker profile not found for this user.");

                // Get sales for the Broker
                var sales = await _salesRepository.GetData();
                var brokerSales = sales.Where(s => s.BrokerId == broker.BrokerId);

                if (!brokerSales.Any())
                    return NotFound("No sales found for this broker.");

                return Ok(_mapper.Map<IEnumerable<SalesDto>>(brokerSales));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving broker sales: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving broker sales.");
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