using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Data.Objects;
using System.Net.Mail;
using Mvc_auction.Models;
using System.Configuration;


    public class UserRepository
    {
        private CustomMembershipDB db = new CustomMembershipDB();

        public MembershipUser CreateUser(string username, string password, string email,string lastName,string name)
        {
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                User user = new User();
                user.userName = username;
                user.mail = email;
                user.lastName = lastName;
                user.name = name;
                user.passwordSalt = CreateSalt();
                user.password = CreatePasswordHash(password, user.passwordSalt);                
                user.createdDate = DateTime.Now;
                user.isActivated = true;
                user.isLockedOut = false;
                user.lastLockedOutDate = DateTime.Now;
                user.lastLoginDate = DateTime.Now;
                // ключ для активации
                user.newMail = GenerateKey();
               // SendMail(user);
                MailSender.SendMail(1, user);
                db.AddToUsers(user); 
                db.SaveChanges();
                
                return GetUser(username);
            }
        }
        public bool ChangePassword(string username)
        {
             bool result = false;
             using (CustomMembershipDB db = new CustomMembershipDB())
             {
                 var user = db.Users.FirstOrDefault(u => u.userName == username);
                 if (user != null)
                 {
                     string newPassword = GeneratePassword();
                     user.newMail = newPassword;
                     user.passwordSalt = CreateSalt();
                     user.password = CreatePasswordHash(newPassword, user.passwordSalt);
                     db.AcceptAllChanges();
              /*       if (db.SaveChanges() > 0)
                     {
                         result = true;
                     }
*/                 }
             }
          return result;
        }
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            bool result=false;
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                var user = db.Users.FirstOrDefault(u => u.userName == username);
                if (user != null)
                {
                    user.passwordSalt = CreateSalt();
                    user.password = CreatePasswordHash(newPassword, user.passwordSalt);
                    user.newMail = null;
                    user.modifyedDate = DateTime.Now;
                    Save();
                   result=true;
                }
            }
            return result;
        }
        public void SendMail(User user)
        {
           string conString= HttpContext.Current.Request.Url.Authority;
            string activationLink = "http://localhost:1595/Account/Activate/" +
                                      user.userName + "/" + user.newMail;

            var message = new MailMessage(ConfigurationManager.AppSettings["MailName"], user.mail) //ConfigurationManager.AppSettings["MailName"]
            {
                Subject = "Activate your account",
                Body = activationLink
            };

            var client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailName"], ConfigurationManager.AppSettings["MailPassword"]); 
            //client.UseDefaultCredentials = false;
            client.Send(message);
        }

        public bool SaveUser(User newUser)
        {
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                var result = from u in db.Users where (u.user_id == newUser.user_id) select u;
                if (result.Count() != 0)
                {
                    var dbuser = result.First();
                    dbuser.comments = newUser.comments;
                    dbuser.mail = newUser.mail;
                    dbuser.name = newUser.name;
                    dbuser.lastName = newUser.lastName;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeleteUser(int id)
        {
            bool result = false;
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                try
                { 
                    db.DeleteObject(db.Users.First(u => u.user_id == id));
                    db.SaveChanges();
                    result=true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }                
            }
            return result;
        }

        public bool ActivateUser(string username, string key)
        {
            bool result = false;
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                var dbuser = db.Users.FirstOrDefault(u => u.userName == username);
                if (dbuser != null)
                {
                    if (dbuser.newMail == key)
                    {
                        dbuser.isActivated = true;
                        dbuser.modifyedDate = DateTime.Now;
                        dbuser.newMail = null;
                        db.SaveChanges();
                        result=true;
                    }
                }
                //    else
                //    {
                //        return false;
                //    }

                //}
                //else
                //{
                return result;
                //}
            }
        }
        public string GetUserNameByEmail(string email)
        {
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
               IQueryable<User>  result = from u in db.Users where (u.mail == email) select u;
                if (result.Count() != 0)
                {
                    var dbuser = result.FirstOrDefault();
                    return dbuser.userName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public MembershipUser GetUser(string username)
        {
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                var result = from u in db.Users where (u.userName == username) select u;

                if (result.Count() != 0)
                {
                    var dbuser = result.FirstOrDefault();
                    string _username = dbuser.userName;
                    int _providerUserKey = dbuser.user_id;
                    string _email = dbuser.mail;
                   // string _lastName = dbuser.lastName;
                  //  string _name = dbuser.name;
                    string _passwordQuestion = "";
                    string _comment = dbuser.comments;
                    bool _isApproved =(bool) dbuser.isActivated;
                    bool _isLockedOut =(bool) dbuser.isLockedOut;
                    DateTime _creationDate =(DateTime) dbuser.createdDate;
                    DateTime _lastLoginDate =(DateTime) dbuser.lastLoginDate;
                    DateTime _lastActivityDate = DateTime.Now;
                    DateTime _lastPasswordChangedDate = DateTime.Now;
                    DateTime _lastLockedOutDate =(DateTime) dbuser.lastLockedOutDate;

                    MembershipUser user = new MembershipUser("CustomMembershipProvider",
                                                              _username,
                                                              _providerUserKey,
                                                              _email,
                                                              _passwordQuestion,
                                                              _comment,
                                                              _isApproved,
                                                              _isLockedOut,
                                                              _creationDate,
                                                              _lastLoginDate,
                                                              _lastActivityDate,
                                                              _lastPasswordChangedDate,
                                                              _lastLockedOutDate);

                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }
        private static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd =
                    FormsAuthentication.HashPasswordForStoringInConfigFile(
                    saltAndPwd, "sha1");
            return hashedPwd;
        }
        private static string GenerateKey()
        {
            Guid emailKey = Guid.NewGuid();
            return emailKey.ToString();
        }
        private static string GeneratePassword()
        {
            string str = Convert.ToString(Guid.NewGuid());
           return str.Replace("-","");
        }
        public bool ValidateUser(string username, string password)
        {
            using (CustomMembershipDB db = new CustomMembershipDB())
            {
                var result = from u in db.Users where (u.userName == username) select u;

                if (result.Count() != 0)
                {
                    var dbuser = result.First();

                    return ((dbuser.password == CreatePasswordHash(password, dbuser.passwordSalt))&&(dbuser.isActivated == true));
                }
                else
                {
                    return false;
                }
            }
        }
       
        public User GetDBUser(string username)
        {
            return db.Users.SingleOrDefault(x => x.userName == username);

        }
        public User GetDBUser(int user_id)
        {
            return db.Users.SingleOrDefault(x => x.user_id == user_id);

        }
       
        public Role GetRole(string name)
        {
            return db.Roles.SingleOrDefault(x => x.name == name);
        }

        public List<User> GetAllUsers()
        {
            return db.Users.ToList();
        }

        public void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (var username in usernames)
            {
                var user = GetDBUser(username);
                if (user != null)
                {
                    foreach (var rolename in rolenames)
                    {
                        var role = GetRole(rolename);
                        if (role != null)
                            if (!user.Roles.Contains(role))
                                user.Roles.Add(role);
                    }
                }
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void CreateRole(string roleName)
        {
            if (GetRole(roleName) == null)
                db.AddToRoles(new Role { name = roleName });
        }

    }
