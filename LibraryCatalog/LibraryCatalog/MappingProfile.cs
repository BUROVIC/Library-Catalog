using System.Linq;
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
            CreateMap<Author, AuthorBriefDto>();

            CreateMap<Publication, PublicationBriefDto>();
            CreateMap<Publication, PublicationDto>()
                .ForMember(
                    dto => dto.AuthorsIds,
                    memberConfiguration => memberConfiguration.MapFrom(
                        publication => publication.Authors.Select(author => author.Id)
                    )
                )
                .ForMember(
                    dto => dto.ReviewsIds,
                    memberConfiguration => memberConfiguration.MapFrom(publication =>
                        publication.Reviews.Select(review => review.Id)
                    )
                )
                .ForMember(
                    dto => dto.PublisherId,
                    memberConfiguration => memberConfiguration.MapFrom(publication => publication.Publisher.Id)
                );

            CreateMap<Publisher, PublisherDto>()
                .ForMember(
                    dto => dto.PublicationsIds,
                    memberConfiguration => memberConfiguration.MapFrom(
                        publisher => publisher.Publications.Select(publication => publication.Id)
                    )
                );

            CreateMap<Review, ReviewDto>()
                .ForMember(
                    dto => dto.PublicationId,
                    memberConfiguration => memberConfiguration.MapFrom(review => review.Publication.Id)
                );
        }
    }
}