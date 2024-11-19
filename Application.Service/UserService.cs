using System.Collections.Generic;
using System.Linq;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Model.Models;
using Application.Common;
using Application.Service.Properties;
using System;

namespace Application.Service
{
    public interface IUserService
    {
        void CreateUser(User user);
        void UpdateUser(User user);        
        IEnumerable<User> GetManagementUsers();
        IEnumerable<User> GetUserList(string type);
        User GetUserById(string id);
        User GetUser(string username);
        User GetUserExcludeMe(string id, string username);
        User GetUserByCode(string code, string excludeUserId = "");
        User GetUserByPhone(string phoneNo);
        User GetUser(string username, string password);
        IEnumerable<User> GetUnSyncUsers();
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetAllCustomers();
        void MarkIsSync(string userIds);
        bool IsInRole(string username, string roleName);
        bool IsVerifiedUser(string userId);
        bool VerifiedUser(string userId);
        bool ActivateUser(string userId);
        bool DeleteUser(string userId);
        bool ChangePassword(string userName, string newPassword);
        void UpdateLastLoginTime(string userId);
        void UpdateUserInfo(User user);
        IEnumerable<User> GetAllUserEmails(string sendTo, int lastLoginDay = 30);
        void Commit();
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }
        
        #region IUserService Members

        public void CreateUser(User user)
        {
            userRepository.Add(user);
            Commit();
        }

        public void UpdateUser(User user)
        {
            userRepository.Update(user);
            Commit();
        }

        //public IEnumerable<User> GetUsers()
        //{
        //    var users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false).OrderByDescending(u => u.CreateDate).ToList();
        //    return users;
        //}

        public IEnumerable<User> GetManagementUsers()
        {
            var users = userRepository.GetMany(r => r.IsManual == true && r.IsDelete != true).OrderBy(u => u.FirstName).ToList();
            return users;
        }

        public IEnumerable<User> GetUnSyncUsers()
        {
            var users = userRepository.GetMany(r => r.IsDelete != true && r.IsSync != true).ToList();
            return users;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = userRepository.GetMany(r => r.IsDelete != true).ToList();
            return users;
        }

        public IEnumerable<User> GetAllCustomers()
        {
            var users = userRepository.GetMany(r => r.IsManual != true && r.IsDelete != true && r.Username != "Guest").OrderBy(u => u.FirstName).ToList();
            return users;
        }

        public void MarkIsSync(string userIds)
        {
            string sql = String.Format("Update Users Set IsSync = 1 where Id IN ({0})", userIds);

            using (var context = new Data.Models.ApplicationEntities())
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        public IEnumerable<User> GetUserList(string type)
        {
            var users = new List<User>();

            if (type.ToLower() == "unverified")
            {
                users = userRepository.GetMany(r => r.IsVerified == false && r.IsDelete == false).OrderByDescending(u => u.CreateDate).ToList();
            }
            else if (type.ToLower() == "deleted")
            {
                users = userRepository.GetMany(r => r.IsDelete == true).OrderByDescending(u => u.CreateDate).ToList();
            }
            else if (type.ToLower() == "inactive")
            {
                users = userRepository.GetMany(r => r.IsActive == true).OrderByDescending(u => u.CreateDate).ToList();
            }

            return users;
        }

        public IEnumerable<User> GetAllUserEmails(string sendTo, int lastLoginDay = 30)
        {
            var users = new List<User>();

            //if (sendTo.ToLower() == "all")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false)
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name } }).ToList();
            //}
            //else if (sendTo.ToLower() == "paidmembers")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false && r.IsBlock == false && r.UserProfile != null && r.UserProfile.Name != null && r.Subscription != null && r.Subscription.ExpireDate >= DateTime.Now)
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name } }).ToList();
            //}
            //else if (sendTo.ToLower() == "nonpaidmembers")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false && r.IsBlock == false && r.UserProfile != null && r.UserProfile.Name != null && (r.Subscription == null || r.Subscription.ExpireDate < DateTime.Now))
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name } }).ToList();
            //}
            //else if (sendTo.ToLower() == "incomplete")
            //{
            //    users = userRepository.GetMany(r => r.UserProfile != null && r.UserProfile.Name == null)
            //        .Select(r => new User { Id = r.Id, Username = r.Username }).ToList();
            //}
            //else if (sendTo.ToLower() == "viewcount")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false && r.IsBlock == false && r.UserProfile != null && r.UserProfile.Name != null && r.UserProfile.ViewCount != null && r.UserProfile.ViewCount > 0)
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name, ViewCount = r.UserProfile.ViewCount } }).ToList();
            //}
            //else if (sendTo.ToLower() == "lastlogin")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false && r.IsBlock == false && r.UserProfile != null && r.UserProfile.Name != null
            //        && (r.LastLoginTime == null || System.Data.Entity.DbFunctions.DiffDays(r.LastLoginTime, DateTime.Now) > lastLoginDay))
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name } }).ToList();               
            //}
            //else if (sendTo.ToLower() == "birthdaywish")
            //{
            //    users = userRepository.GetMany(r => r.IsActive == true && r.IsVerified == true && r.IsDelete == false && r.IsBlock == false && r.UserProfile != null && r.UserProfile.Name != null
            //        && (((DateTime)r.UserProfile.DateOfBirth).Month == DateTime.Now.Month && ((DateTime)r.UserProfile.DateOfBirth).Day == DateTime.Now.Day))
            //        .Select(r => new User { Id = r.Id, Username = r.Username, UserProfile = new UserProfile { Name = r.UserProfile.Name } }).ToList();
            //}

            return users;
        }

        public User GetUserById(string id)
        {
            var users = userRepository.Get(u => u.Id == id);
            return users;
        }

        public User GetUser(string username)
        {
            var users = userRepository.Get(u => u.Username == username || u.Email == username);
            return users;
        }

        public User GetUserExcludeMe(string id, string username)
        {
            var users = userRepository.Get(u => u.Id != id && u.Username == username);
            return users;
        }

        public User GetUserByCode(string code, string excludeUserId = "")
        {
            User user = null;
            if (String.IsNullOrEmpty(excludeUserId))
            {
                user = userRepository.Get(u => u.Code == code);
            }
            else
            {
                user = userRepository.Get(u => u.Id != excludeUserId && u.Code == code);
            }

            return user;
        }

        public User GetUserByPhone(string phoneNo)
        {
            var users = userRepository.Get(u => u.Username == phoneNo);
            return users;
        }

        public User GetUser(string username, string password)
        {
            var users = userRepository.Get(u => (u.Email == username || u.Username == username) && u.Password == password && u.IsActive == true && u.IsDelete == false && !u.IsWholeSaler);
            return users;
        }

        public bool IsInRole(string username, string roleName)
        {
            bool isInRole = false;

            var user = userRepository.Get(u => u.Username == username || u.Email == username);
            if (user != null)
            {
                if (user.Roles != null)
                {
                    foreach (Role role in user.Roles)
                    {
                        if (role.Name == roleName)
                        {
                            isInRole = true;
                            break;
                        }
                    }
                }
            }

            return isInRole;
        }

        public bool IsVerifiedUser(string userId)
        {
            var user = userRepository.Get(u => u.Id == userId && u.IsActive == true && u.IsVerified == true);
            if (user != null)
            {               
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifiedUser(string userId)
        {
            var user = userRepository.Get(u => u.Id == userId);
            if (user != null)
            {
                user.IsVerified = true;
                userRepository.Update(user);
                Commit();
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool DeleteUser(string userId)
        {
            var user = userRepository.Get(u => u.Id == userId);
            if (user != null)
            {
                user.IsDelete = true;
                userRepository.Update(user);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ActivateUser(string userId)
        {
            var user = userRepository.Get(u => u.Id == userId);
            if (user != null)
            {
                user.IsActive = true;
                userRepository.Update(user);
                Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ChangePassword(string userName, string newPassword)
        {
            User user = this.GetUser(userName);
            if (user != null)
            {
                user.Password = newPassword;
            }
            else
            {
                return false;
            }
            
            userRepository.Update(user);
            Commit();

            return true;
        }

        public void UpdateLastLoginTime(string userId)
        {
            User user = this.GetUserById(userId);
            if (user != null)
            {
                user.LastLoginTime = DateTime.Now;

                userRepository.Update(user);
                Commit();
            }            
        }

        public void UpdateUserInfo(User user)
        {
            userRepository.Update(user);
            Commit();
        }

        public void Commit()
        {
            unitOfWork.Commit();
        }
              
        #endregion
    }
}
