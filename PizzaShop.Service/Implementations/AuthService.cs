using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using System.Text.Json;

namespace PizzaShop.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private IUserRepository  _userRepository;

    private readonly ApplicationDbContext _dbo;

    public AuthService(IConfiguration configuration, IUserRepository userRepository, ApplicationDbContext dbo)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _dbo = dbo;
    }

    public async Task<UserViewModel> Login(string email, string password)
    {
        User user = await _userRepository.CheckUserEmailAndPassword(email, password);
        if(user == null)
        {
            return null;
        }
        Role role = _userRepository.GetRoleById(user.Roleid);
        if (user == null)
        {
            return null;
        }
        UserViewModel model = new UserViewModel{
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Phone = user.Phone,
            Rolename = role.Rolename,
            RoleId = user.Roleid,
            Country = user.Country,
            State = user.States,
            City = user.City,
            Zipcode = user.Zipcode,
            Address = user.Address,
            Profileimagepath = user.Profileimagepath
        };
        return model;
    }

    public string GenerateJwtToken(string email, int? RoleId)
    {
        var Rolename = _dbo.Roles.Where(r => r.Roleid == RoleId).Select(r => r.Rolename).FirstOrDefault();
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, Rolename)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(360),
            // Expires = DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateJwtTokenForgot(User user, bool rememberMe)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("dhfdjnfbvdkjcndskfhjuiejedhk#4243hdbdjcnsdsfregrefeds3rewsdsfdssdfsdfsskoik");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage){
        var emailToSend = new MimeMessage();
        emailToSend.From.Add(MailboxAddress.Parse("test.dotnet@etatvasoft.com"));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){Text = htmlMessage};

        using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
        {
            emailClient.Connect("mail.etatvasoft.com", 587, SecureSocketOptions.StartTls);
            emailClient.Authenticate("test.dotnet@etatvasoft.com", "P}N^{z-]7Ilp");
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }

        await Task.CompletedTask;
    }

    public bool CheckEmailExist(string email)
    {
        var user =  _userRepository.GetUserByEmail(email);
        if(user != null)
        {
            return true;
        }
        return false;
    }



}
