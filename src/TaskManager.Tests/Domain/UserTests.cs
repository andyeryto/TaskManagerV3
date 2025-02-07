using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Create_ShouldThrowException_WhenUsernameIsNull()
        {
            Action act = () => User.Create(null!, "securepassword");
            act.Should().Throw<ArgumentNullException>().WithMessage("*username*");
        }

        [Fact]
        public void Create_ShouldThrowException_WhenPasswordIsNull()
        {
            Action act = () => User.Create("JohnDoe", null!);
            act.Should().Throw<ArgumentNullException>().WithMessage("*rawPassword*");
        }

        [Fact]
        public void Create_ShouldHashPassword_Correctly()
        {
            var user = User.Create("JohnDoe", "securepassword");
            user.PasswordHash.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Create_ShouldGenerateUniqueId()
        {
            var user1 = User.Create("User1", "pass1");
            var user2 = User.Create("User2", "pass2");

            user1.Id.Should().BePositive();
            user2.Id.Should().NotBe(user1.Id);
        }
    }
}
