using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                        .Include(p => p.Photos)
                        .ToListAsync();

        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsenameAsync(string username)
        {
            return await _context.Users
                           .Include(p => p.Photos)
                           .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                        .AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);

            //let's say user was looking for someone bet 20 and 30, than max age here
            //will be 30 and min  age would be here is 20
            //
            //here MaxAge is 30 and we will subtract(no of years - 1) from current datetime's year 
            //if year is 2020
            //1989 will be minDob
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            //here MaxAge is 20  and we will subtract no of years from current datetime's year
            //if year is 2020
            //2000 will be maxDob
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            //1989 to 2020
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //sorting
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                //default case below
                _ => query.OrderByDescending(u => u.LastActive)
            };

            var queryBuild = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreatAsync(queryBuild, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                        .Where(x => x.UserName == username)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();
        } 
    }
}
