using AutoMapper;
using MRA.Pages.Application.Contract.Content.Commands;
using MRA.Pages.Application.Contract.Content.Responses;

namespace MRA.Pages.Application.Features;

public class ContentProfile : Profile
{
    public ContentProfile()
    {
        CreateMap<Domain.Entities.Content, ContentResponse>()
            .ForMember(s => s.PageName, op =>
                op.MapFrom(f => f.Page == null ? "" : f.Page.Name));

        CreateMap<CreateContentCommand, Domain.Entities.Content>();
        CreateMap<ContentResponse, UpdateContentCommand>();
    }
}