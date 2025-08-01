﻿using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServicePriceRepository _priceRepository;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository requestRepository, 
            ICustomerRepository customerRepository, IMapper mapper, IServicePriceRepository priceRepository)
        {
            _requestRepository = requestRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _priceRepository = priceRepository;
        }

        public async Task<IEnumerable<RequestDto>> GetRequestsAsync()
        {
            var requests = await _requestRepository.GetRequestsAsync();
            return _mapper.Map<IEnumerable<RequestDto>>(requests);
        }

        public async Task<RequestDto?> GetRequestByIdAsync(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null) return null;

            var dto = _mapper.Map<RequestDto>(request);

            if (request.Service != null)
            {
                dto.ServiceType = request.Service.ServiceType;
                dto.ServicePrice = request.Service.Price;
                dto.ServiceDuration = request.Service.Duration;
                dto.ServiceDescription = request.Service.Description;
            }

            if (request.Employee?.User != null)
            {
                dto.EmployeeName = $"{request.Employee.User.FirstName} {request.Employee.User.LastName}".Trim();
            }

            return dto;
        }

        public async Task<IEnumerable<RequestDto>> GetRequestsByCustomerIdAsync(string userId)
        {
            var requests = await _requestRepository.GetRequestsByCustomerIdAsync(userId);
            return _mapper.Map<IEnumerable<RequestDto>>(requests);
        }

        public async Task<List<RequestDto>> GetDraftOrPendingRequestsAsync(string userId)
        {
            var requests = await _requestRepository.GetRequestsByCustomerAsync(userId);

            var draftRequests = requests
                .Where(r => r.Status == "Draft" || r.Status == "Pending")
                .ToList();

            return _mapper.Map<List<RequestDto>>(draftRequests);
        }

        public async Task<List<RequestWithServiceDto>> GetDraftOrPendingRequestsWithServiceAsync(string userId)
        {
            var requests = await _requestRepository.GetDraftOrPendingRequestsAsync(userId);

            return requests
                .Where(r => r.Service != null)
                .Select(r => new RequestWithServiceDto
                {
                    RequestId = r.RequestId,
                    RequestType = r.RequestType,
                    RequestDate = r.RequestDate,
                    ServiceId = r.Service.ServiceId,
                    ServiceType = r.Service.ServiceType,
                    Price = r.Service.Price,
                    Duration = r.Service.Duration,
                    Description = r.Service.Description,
                    Status = r.Service.Status
                })
                .ToList();
        }

        /// <summary>
        /// Unified method to create Draft or Official request for a Customer
        /// </summary>
        public async Task<RequestDto?> CreateRequestForCustomerAsync(string userId, RequestCreateDto dto, string status)
        {
            var customerId = await _customerRepository.GetCustomerIdAsync(userId);
            if (customerId == -1)
                return null;

            var request = _mapper.Map<Request>(dto);
            request.CustomerId = customerId;
            request.Status = status;
            request.RequestDate = DateTime.UtcNow;

            var created = await _requestRepository.CreateRequestAsync(request);
            return _mapper.Map<RequestDto>(created);
        }

        public async Task<bool> CancelRequestAsync(string userId, int requestId)
        {
            return await _requestRepository.CancelRequestAsync(userId, requestId);
        }

        public async Task<bool> UpdateRequestAsync(int id, RequestCreateDto updateDto)
        {
            var existingRequest = await _requestRepository.GetRequestByIdAsync(id);
            if (existingRequest == null)
                return false;

            if (existingRequest.Status != "Draft")
            {
                return false; 
            }

            _mapper.Map(updateDto, existingRequest);
            return await _requestRepository.UpdateRequestAsync(existingRequest);
        }

        public async Task<bool> UpdateRequestStatusAsync(int requestId, string newStatus)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null) return false;

            request.Status = newStatus;
            return await _requestRepository.UpdateRequestAsync(request);
        }
    }
}
