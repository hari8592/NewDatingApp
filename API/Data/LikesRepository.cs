using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            //these 2 filelds are primary key 
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        //we will ask  if stmsts and conditions inside here to return different body of list
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(a => a.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
                //gives us users from likes table
                users = likes.Select(l => l.LikedUser);
            }
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(l => l.LikedUserId == likesParams.UserId);
                //gives us users from likes table
                users = likes.Select(l => l.SourceUser);
            }
            var likedUsers= users.Select(user => new LikeDto
            {
                UserName=user.UserName,
                KnownAs=user.KnownAs,
                Age=user.DateOfBirth.CalculateAge(),
                PhotoUrl=user.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City=user.City,
                Id=user.Id

            });
            return await PagedList<LikeDto>.CreatAsync(likedUsers,likesParams.PageNumber,likesParams.PageSize);
        }

        //get list of  users that -this user has liked and that we get -users liked the other users
        public async Task<AppUser> GetUsersWithLikes(int userId)
        {
            return await _context.Users
                        .Include(x => x.LikedUsers)
                        .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
