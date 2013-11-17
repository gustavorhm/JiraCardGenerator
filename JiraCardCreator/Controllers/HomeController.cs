using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using JiraCardCreator.ViewModels;
using Newtonsoft.Json;

namespace JiraCardCreator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateCards(string username, string password, string url, string queryString)
        {
            HttpClient client = PrepareHttpClient(username, password, url);
            HttpResponseMessage response = client.GetAsync(queryString).Result;

            var issues = new List<Issue>();
            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                dynamic issuesList = jsonResponse.issues;
                
                for (int i = 0; i != issuesList.Count; i++)
                {
                    issues.Add(new Issue { Id = issuesList[i].key.ToString(), Name = issuesList[i].fields["summary"].ToString(), Description = issuesList[i].fields["description"].ToString(), Priority = issuesList[i].fields["priority"]["id"].ToString(), StoryPoints = issuesList[i].fields["customfield_10073"] });
                }
            }

            return Json(issues);
        }

        private HttpClient PrepareHttpClient(string username, string password, string jiraUrl)
        {
            var client = new HttpClient { BaseAddress = new Uri(jiraUrl + "/rest/api/2/search") };

            byte[] cred = Encoding.UTF8.GetBytes(username + ":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return client;
        }
    }
}