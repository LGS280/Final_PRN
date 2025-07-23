using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using PhoneNumbers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiamondAssessmentSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(string userId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(userId);
            return customer == null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> UpdateCustomerAsync(string userId, CustomerCreateDto dto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(userId);
            if (customer == null) return false;

            // Cập nhật thông tin user
            customer.User.FirstName = dto.FirstName;
            customer.User.LastName = dto.LastName;
            customer.User.PhoneNumber = dto.Phone;
            customer.User.Gender = dto.Gender;

            customer.Address = dto.Address;
            customer.Idcard = string.IsNullOrEmpty(dto.IdCard) ? null : decimal.Parse(dto.IdCard);
            customer.UnitName = dto.UnitName;
            customer.TaxCode = dto.TaxCode;

            return await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task<bool> UpdateCustomerAsync(CustomerUpdateDtoV2 dto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(dto.UserId);
            if (customer == null) return false;

            if (!IsPhoneNumberValid(dto.Phone, "VN"))
            {
                throw new Exception($"Phone is invalid!");

            } 

            var user = customer.User;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.Phone;
            user.Gender = dto.Gender;
            user.Email = dto.Email;
            user.UserName = dto.UserName;

            customer.Address = dto.Address;
            customer.Idcard = string.IsNullOrEmpty(dto.IdCard) ? null : decimal.Parse(dto.IdCard);
            customer.UnitName = dto.UnitName;
            customer.TaxCode = dto.TaxCode;

            return await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task<bool> DeleteCustomerAsync(string userId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(userId);
            if (customer == null) return false;

            await _userRepository.DeleteUserAsync(userId);
            return true;
        }

        private bool IsPhoneNumberValid(string phoneNumber, string regionCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneNumberUtil.Parse(phoneNumber, regionCode);
                return phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
