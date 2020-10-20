using AutoMapper;
using LibraryCatalog.Data.Entities;
using LibraryCatalog.Models;

namespace LibraryCatalog
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDto>();
            CreateMap<AuthorDto, Author>();
        }
    }
}