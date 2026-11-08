using AutoMapper;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Kullanicilar, KullaniciDto>();

            CreateMap<KullaniciCreateDto, Kullanicilar>(
                MemberList.None)
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ParolaHash,
                    opt => opt.Ignore());

            CreateMap<KullaniciUpdateDto, Kullanicilar>(
                MemberList.None)
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.ParolaHash,
                    opt => opt.Ignore());

            CreateMap<Tasinmazlar, TasinmazDto>()
                .ForMember(
                    dest => dest.MahalleAdi,
                    opt => opt.MapFrom(src => src.Mahalle.MahalleAdi))
                .ForMember(
                    dest => dest.IlceAdi,
                    opt => opt.MapFrom(src =>
                        src.Mahalle.Ilce.IlceAdi))
                .ForMember(
                    dest => dest.IlAdi,
                    opt => opt.MapFrom(src =>
                        src.Mahalle.Ilce.Il.IlAdi));

            CreateMap<TasinmazCreateDto, Tasinmazlar>(MemberList.None)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.KullaniciId, opt => opt.Ignore())
                .ForMember(dest => dest.KayitTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Kullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Mahalle, opt => opt.Ignore());

            CreateMap<TasinmazUpdateDto, Tasinmazlar>(MemberList.None)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.KullaniciId, opt => opt.Ignore())
                .ForMember(dest => dest.Enlem, opt => opt.Ignore())
                .ForMember(dest => dest.Boylam, opt => opt.Ignore())
                .ForMember(dest => dest.KayitTarihi, opt => opt.Ignore())
                .ForMember(dest => dest.Kullanici, opt => opt.Ignore())
                .ForMember(dest => dest.Mahalle, opt => opt.Ignore());


            CreateMap<Loglar, LogDto>()
                .ForMember(
                    dest => dest.DurumAdi,
                    opt => opt.MapFrom(src =>
                        src.Durum.DurumAdi))
                .ForMember(
                    dest => dest.IslemTipiAdi,
                    opt => opt.MapFrom(src =>
                        src.IslemTipi.IslemTipiAdi));

        }
    }
}
