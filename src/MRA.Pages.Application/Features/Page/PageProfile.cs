using AutoMapper;
using MRA.Pages.Application.Contract.Page.Commands;
using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Features.Page;

public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<CreatePageCommand, Domain.Entities.Page>();
        CreateMap<UpdatePageCommand, Domain.Entities.Page>();
        CreateMap<Domain.Entities.Page, PageResponse>();
    }
}