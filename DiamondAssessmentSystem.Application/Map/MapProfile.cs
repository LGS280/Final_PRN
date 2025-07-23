using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Application.Map
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));

            CreateMap<RegisterEmployeesDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));

            CreateMap<User, AccountDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<Request, RequestDto>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.Service.ServiceType))
                .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.ServiceDuration, opt => opt.MapFrom(src => src.Service.Duration))
                .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => src.Service.Description))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.User != null
                        ? src.Employee.User.FirstName + " " + src.Employee.User.LastName
                        : null));

            CreateMap<RequestDto, Request>();

            CreateMap<Request, RequestCreateDto>().ReverseMap();

            CreateMap<ServicePrice, ServicePriceCreateDto>().ReverseMap();
            CreateMap<ServicePrice, ServicePriceDto>().ReverseMap();
            CreateMap<ServicePrice, ServicePriceDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();

            CreateMap<Result, ResultDto>().ReverseMap();
            CreateMap<Result, ResultCreateDto>().ReverseMap();

            CreateMap<CustomerCreateDto, Customer>()
                .ForMember(dest => dest.Idcard, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.IdCard) ? (decimal?)null : decimal.Parse(src.IdCard)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitName))
                .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode));

            CreateMap<CustomerUpdateDtoVer1, CustomerCreateDto>().ReverseMap();


            CreateMap<CustomerUpdateDtoVer1, Customer>()
                .ForMember(dest => dest.Idcard, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.IdCard) ? (decimal?)null : decimal.Parse(src.IdCard)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitName))
                .ForPath(dest => dest.User.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode));

            CreateMap<CustomerCreateDto, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.User.Point))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.User.Note))
                .ForMember(dest => dest.IdCard, opt => opt.MapFrom(src => src.Idcard.ToString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitName))
                .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode))
                .ForMember(dest => dest.Acc, opt => opt.MapFrom(src => src.User));

            CreateMap<User, AccountDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status));

            CreateMap<EmployeeDto, Employee>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<EmployeeUpdateDto, EmployeeDto>().ReverseMap();


            CreateMap<Certificate, CertificateDto>().ReverseMap();
            CreateMap<Certificate, CertificateCreateDto>().ReverseMap();

            CreateMap<BlogDto, Blog>()
    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Status) ? "Draft" : src.Status));

            CreateMap<Blog, BlogDto>();

            CreateMap<Conversation, ConversationDTO>();

            CreateMap<ChatLog, ChatLogDTO>();
            CreateMap<ChatLog, MessageResponseDTO>()
                .ForMember(dest => dest.SenderRole, opt => opt.MapFrom(src => src.SenderRole.ToString()))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType.ToString()));
            CreateMap<CreateMessageDTO, ChatLog>();

            CreateMap<ChatLogDTO, MessageResponseDTO>();

            CreateMap<OrderCreateDto, Order>().ReverseMap();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.Service.ServiceType));

            CreateMap<(string CustomerName, int RequestCount), ReportTopCustomerDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.RequestCount, opt => opt.MapFrom(src => src.RequestCount));

            CreateMap<(string Status, int RequestCount), ReportRequestStatusDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RequestCount, opt => opt.MapFrom(src => src.RequestCount));
        }
    }
}
