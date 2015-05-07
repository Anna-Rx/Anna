using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anna.Json;
using Anna.Tests.Json.Models;
using Anna.Tests.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace Anna.Tests.Json
{
    public class GetAsTests
    {
        [Test]
        public void GetAsReturnsInstanceFromPostData()
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

                server.POST("/Person").Subscribe(ctx =>
                                                 {
                                                     var requestPerson = ctx.GetAs<PersonModel>();

                                                     requestPerson.Should().Not.Be.Null();
                                                     requestPerson.Should().Be.EqualTo(person);

                                                     ctx.Respond("success");
                                                 });

                Browser.ExecutePostAsJson("http://localhost:1234/Person", person)
                    .ReadAllContent()
                    .Should().Be("success");
            }
        }

        [Test]
        public void GetAsReturnsDataFromPostAndUriArguements()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                var person = new PersonModel
                             {
                                 DateOfBirth = new DateTime(1975, 1, 1),
                                 FirstName = "Shawn",
                                 LastName = "Spencer"
                             };

                server.POST("/Person/{PersonId}").Subscribe(ctx =>
                                                            {
                                                                var requestPerson = ctx.GetAs<PersonModel>();

                                                                requestPerson.Should().Not.Be.Null();

                                                                requestPerson.PersonId.Should().Be(6789);

                                                                ctx.Respond("success");
                                                            });

                Browser.ExecutePostAsJson("http://localhost:1234/Person/6789", person)
                    .ReadAllContent()
                    .Should().Be("success");
            }
        }
    }
}
