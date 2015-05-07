using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anna.Tests.Json.Models;
using Anna.Tests.Util;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpTestsEx;
using Anna.Json;

namespace Anna.Tests.Json
{
    public class RespondAsTests
    {
        [Test]
        public void RespondAsWithClass()
        {
            using (var server = new HttpServer("http://*:1234/"))
            {
                var person = new PersonModel
                             {
                                 DateOfBirth = new DateTime(1975,1,1),
                                 FirstName = "Shawn",
                                 LastName = "Spencer",
                                 PersonId = 1234
                             };

                server.GET("/").Subscribe(ctx => ctx.RespondAs(person));

                var content = Browser.ExecuteGet("http://localhost:1234").ReadAllContent();

                var secondPerson = JsonConvert.DeserializeObject<PersonModel>(content);

                secondPerson.Should().Not.Be.Null();

                secondPerson.Should().Be.EqualTo(person);
            }
        }

    }
}
