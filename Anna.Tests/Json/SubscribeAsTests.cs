using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anna.Json;
using Anna.Tests.Json.Models;
using Anna.Tests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpTestsEx;

namespace Anna.Tests.Json
{
    public class SubscribeAsTests
    {
        [Test]
        public void PostSubscribeAsPersonModel()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                var person = new PersonModel
                             {
                                 DateOfBirth = new DateTime(1975, 1, 1),
                                 FirstName = "Shawn",
                                 LastName = "Spencer",
                                 PersonId = 1234
                             };

                server.POST("/Person").SubscribeAs<PersonModel>(model => model.Should().Be.EqualTo(person));

                Browser.ExecutePostAsJson("http://localhost:1234/Person", person).ReadAllContent();
            }
        }

        [Test]
        public void GetSubscribeAsPersonModelFromQueryString()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("/Person").SubscribeAs<PersonModel, PersonModel>(model => model);
                
                var body = Browser.ExecuteGet(
                    "http://localhost:1234/Person?FirstName=Shawn&LastName=Spencer&DateOfBirth=1/1/1975&PersonId=1234")
                    .ReadAllContent();

                PersonModel personModel = JsonConvert.DeserializeObject<PersonModel>(body);

                personModel.FirstName.Should().Be("Shawn");
                personModel.LastName.Should().Be("Spencer");
                personModel.DateOfBirth.Should().Be(new DateTime(1975,1,1));
                personModel.PersonId.Should().Be(1234);
            }
        }

        [Test]
        public void GetSubscribeAsPersonModelFromQueryStringAndUriArgs()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                server.GET("/Person/{PersonId}").SubscribeAs<PersonModel, PersonModel>(model => model);

                var body = Browser.ExecuteGet(
                    "http://localhost:1234/Person/1234?FirstName=Shawn&LastName=Spencer&DateOfBirth=1/1/1975")
                    .ReadAllContent();

                PersonModel personModel = JsonConvert.DeserializeObject<PersonModel>(body);

                personModel.FirstName.Should().Be("Shawn");
                personModel.LastName.Should().Be("Spencer");
                personModel.DateOfBirth.Should().Be(new DateTime(1975, 1, 1));
                personModel.PersonId.Should().Be(1234);
            }
        }
    }
}
