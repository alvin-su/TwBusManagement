using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tw.Bus.Entity;
using Tw.Bus.EntityDTO;

namespace Tw.Bus.WebApi
{
    public class TwBusDataMapper
    {
        public TwBusDataMapper()
        {
        }

        public static void InitializeDto()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<Usy_User, UserDto>().
                //ForMember(des => des.UserId, options => options.MapFrom(src => src.Id))
                //.ForMember(des => des.UserName, options => options.MapFrom(src => src.Name));
                cfg.CreateMap<Usy_User, UserDto>();
                cfg.CreateMap<UserDto, Usy_User>();

                cfg.CreateMap<Usy_Menu, MenuDto>();
                cfg.CreateMap<MenuDto, Usy_Menu>();

            });


        }
    }
}
